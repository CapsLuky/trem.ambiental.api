using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Enum;
using API.TrenAmbiental.DTO.Model.Autenticacao;
using API.TrenAmbiental.DTO.ViewModel.Cadastro;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.TrenAmbiental.Domain.Services
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly INotificador _notificador;
        private readonly IEmailService _emailService;
        private readonly ILogger<AutenticacaoService> _logger;
        private readonly IAutenticacaoRepository _autenticacaoRepository;
        private readonly ICadastroRepository _cadastroRepository;
        private static AppSettings _appSettings;

        public AutenticacaoService(
                                   IEmailService emailService,
                                   INotificador notificador,
                                   ILogger<AutenticacaoService> logger,
                                   IOptions<AppSettings> appSettings,
                                   IAutenticacaoRepository autenticacaoRepository,
                                   ICadastroRepository cadastroRepository)
        {
            _autenticacaoRepository = autenticacaoRepository;
            _cadastroRepository = cadastroRepository;
            _emailService = emailService;
            _notificador = notificador;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public async Task<bool> AlterarSenha(AlterarSenhaModel alterarSenha)
        {
            try
            {
                var senhaNovaCriptografada = CriptografarSenha(alterarSenha.SenhaNova);
                
                var senhaAlteradaComSucesso =
                    await _autenticacaoRepository.AtualizarSenha(alterarSenha.Email, senhaNovaCriptografada);
                
                return senhaAlteradaComSucesso;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao alterar senha, Email: {alterarSenha.Email} - {e.Message}", e);
                return false;
            }
        }

        public string CriptografarSenha(string senha)
        {
            var senhaCriptografada = BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 13);
            return senhaCriptografada;
        }

        public async Task<UsuarioInfoViewModel> BuscarInfoUsuario(string email)
        {
            try
            {
                var usuarioInfo = await _cadastroRepository.BuscarUsuarioPorEmail(email);
                return usuarioInfo;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Tivemos um erro inesperado",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao buscar usuario info, Email: {email} - {e.Message}", e);
                throw;
            }
        }

        public async Task<bool> ChecarUsuarioSenha(string email, string senha)
        {
            try
            {
                var senhaAtual = await _autenticacaoRepository.BuscarSenha(email);

                if (string.IsNullOrEmpty(senhaAtual))
                    return false;

                return BCrypt.Net.BCrypt.Verify(senha, senhaAtual);
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao checar usuario e senha, Email: {email} - {e.Message}", e);
                throw;
            }
        }

        public async Task<bool> ChecarUsuarioAtivo(string email)
        {
            try
            {
                var ativo = await _autenticacaoRepository.ChecarUsuarioAtivo(email);
                
                if (ativo == 0)
                {
                    _notificador.Handle(new Notificacao("Erro", "Email ou senha incorretos",
                        ETipoNotificacao.error.ToString()));
                }

                return Convert.ToBoolean(ativo);
                
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao verificar usuário e senha",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao verificar usuário e senha, Email: {email} - {e.Message}", e);
                throw;
            }
        }

        public UsuarioInfoViewModel GerarJwt(UsuarioInfoViewModel usuarioInfo)
        {
            var subject = new ClaimsIdentity(
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuarioInfo.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, usuarioInfo.Email),
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //ID do token
                    new(JwtRegisteredClaimNames.Nbf, ConverterParaDataUnix(DateTime.UtcNow)
                        .ToString()), // Não é válido antes de...
                    new Claim(JwtRegisteredClaimNames.Iat, ConverterParaDataUnix(DateTime.UtcNow).ToString(),
                        ClaimValueTypes.Integer64), // Quando foi emitido
                    new Claim(ClaimTypes.Name, usuarioInfo.Nome),
                    new Claim(ClaimTypes.Role, usuarioInfo.IdValorPerfil.ToString())
                });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = subject,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);

            usuarioInfo.AccessToken = encodedToken;
            usuarioInfo.ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds;

            return usuarioInfo;
        }
        
        public string ValidarToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var email = jwtToken.Claims.First(x => x.Type == "email").ToString().Replace("email: ", "");
                return email;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao validar token. {e.Message}", e);
                return null;
            }
        }
        
        private static long ConverterParaDataUnix(DateTime date)
        {
            return (long) Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);
        }

        public void EnviarEmailComTokenParaResetarSenha(string email)
        {
            try
            {
                var token = GerarJwtRecuperarEmail(email);
                const string body = "Caso tenha pedido para recuperar a senha, clique no link " +
                                    "http://app.tren.eco.br/login/resetar-senha/";
                const string assunto = "Recuperação de senha Tren Ambiental";

                _emailService.EnviarEmail(email, assunto, body, token);
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao enviar email", ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao enviar email. {email}. {e.Message}", e);
                throw;
            }
        }

        public void EnviarEmailComNovaSenha(string email, string novaSenha)
        {
            try
            {
                const string body = "Você solicitou para trocar sua senha, sua nova senha é: ";
                const string assunto = "Recuperação de senha Tren Ambiental";
                _emailService.EnviarEmail(email, assunto, body, novaSenha);
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao enviar email", ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao enviar email. {email}. {e.Message}", e);
                throw;
            }
        }

        private static string GerarJwtRecuperarEmail(string email)
        {
            var subject = new ClaimsIdentity(
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //ID do token
                    new(JwtRegisteredClaimNames.Nbf, ConverterParaDataUnix(DateTime.UtcNow)
                        .ToString()), // Não é válido antes de...
                    new Claim(JwtRegisteredClaimNames.Iat, ConverterParaDataUnix(DateTime.UtcNow).ToString(),
                        ClaimValueTypes.Integer64), // Quando foi emitido
                });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = subject,
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);
            return encodedToken;
        }

        public string GerarSenhaAleatoria()
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");

            var clsRan = new Random();
            var tamanhoSenha = clsRan.Next(6, 6);

            var senha = "";
            for (var i = 0; i <= tamanhoSenha; i++) senha += guid.Substring(clsRan.Next(1, guid.Length), 1);

            return senha;
        }
    }
}