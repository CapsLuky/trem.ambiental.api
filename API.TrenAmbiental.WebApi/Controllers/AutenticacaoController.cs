using System.Threading.Tasks;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Model.Autenticacao;
using API.TrenAmbiental.DTO.ViewModel.Cadastro;
using API.TrenAmbiental.WebApi.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.TrenAmbiental.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AutenticacaoController : MainController
    {
        private static IAutenticacaoService _autenticacaoService;
        private static AppSettings _appSettings;

        public AutenticacaoController(IAutenticacaoService autenticacaoService,
            IOptions<AppSettings> appSettings,
            INotificador notificador) : base(notificador)
        {
            _autenticacaoService = autenticacaoService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioInfoViewModel>> Login([FromBody] LoginModel loginInbound)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var usuarioAtivo = await _autenticacaoService.ChecarUsuarioAtivo(loginInbound.Email);

            if (!usuarioAtivo)
            {
                NotificarErro("Erro", "Sua conta não está ativa");
                return CustomResponse(401);
            }

            var usuarioSenhaValido = await _autenticacaoService.ChecarUsuarioSenha(loginInbound.Email, loginInbound.Senha);
            if (!usuarioSenhaValido)
            {
                NotificarErro("Erro", "Usuário ou Senha incorretos");
                return CustomResponse(401);
            }

            var usuarioInfo = await _autenticacaoService.BuscarInfoUsuario(loginInbound.Email);
            var usuarioComToken = _autenticacaoService.GerarJwt(usuarioInfo);
            return CustomResponse(usuarioComToken);
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPost("login/alterarsenha")]
        public async Task<ActionResult<UsuarioInfoViewModel>> AlterarSenha([FromBody] AlterarSenhaModel altSenha)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var usuarioSenhaValido = await _autenticacaoService.ChecarUsuarioSenha(altSenha.Email, altSenha.SenhaAtual);
            if (!usuarioSenhaValido)
            {
                NotificarErro("Erro", "Senha atual incorreta");
                return CustomResponse(401);
            }

            var usuario = await _autenticacaoService.AlterarSenha(altSenha);
            return CustomResponse(usuario);
        }

        [AllowAnonymous]
        [HttpGet("login/recriarSenha/{email:required}")]
        public async Task<ActionResult> RecriarSenha(string email)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var usuarioInfo = await _autenticacaoService.BuscarInfoUsuario(email);
            if (usuarioInfo == null) return CustomResponse();

            var novaSenha = _autenticacaoService.GerarSenhaAleatoria();
            var novaSenhaObj = new AlterarSenhaModel
                { Email = email, SenhaAtual = "", SenhaNova = novaSenha, TokenRedefinirSenha = "" };
            var senhaAlterada = await _autenticacaoService.AlterarSenha(novaSenhaObj);
            if (senhaAlterada) _autenticacaoService.EnviarEmailComNovaSenha(email, novaSenha);
            return CustomResponse();
        }
        
        [AllowAnonymous]
        [HttpGet("login/enviarEmailRecuperarsenha/{email:required}")]
        public async Task<ActionResult> RecuperarSenha(string email)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var usuarioInfo = await _autenticacaoService.BuscarInfoUsuario(email);
            if (usuarioInfo == null)
            {
                return CustomResponse();
            }

            _autenticacaoService.EnviarEmailComTokenParaResetarSenha(email);

            return CustomResponse();
        }

        /// <summary>
        ///     Valida um token gerado na redefinição de senha
        /// </summary>
        [AllowAnonymous]
        [HttpGet("login/validarToken/{token:required}")]
        public ActionResult<string> ValidarToken(string token)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var tokenValido = _autenticacaoService.ValidarToken(token);

            return CustomResponse(tokenValido);
        }
        
        [AllowAnonymous]
        [HttpPost("login/redefinirSenha")]
        public async Task<ActionResult<bool>> RedefinirSenha([FromBody] AlterarSenhaModel altSenha)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var tokenValido = _autenticacaoService.ValidarToken(altSenha.TokenRedefinirSenha);

            if (string.IsNullOrEmpty(tokenValido)) return CustomResponse(false);
            
            var senhaAlterada = await _autenticacaoService.AlterarSenha(altSenha);
            return CustomResponse(senhaAlterada);

        }
    }
}