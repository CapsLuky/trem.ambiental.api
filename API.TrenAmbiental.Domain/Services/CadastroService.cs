using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Enum;
using API.TrenAmbiental.DTO.Model;
using API.TrenAmbiental.DTO.Model.Cadastro;
using API.TrenAmbiental.DTO.ViewModel.Cadastro;
using Microsoft.Extensions.Logging;

namespace API.TrenAmbiental.Domain.Services
{
    public class CadastroService : ICadastroService
    {
        private readonly IAutenticacaoService _autenticacaoService;
        private readonly INotificador _notificador;
        private readonly ILogger<CadastroService> _logger;
        private readonly ICadastroRepository _cadastroRepository;

        public CadastroService(
            IAutenticacaoService autenticacaoService,
            INotificador notificador,
            ILogger<CadastroService> logger,
            ICadastroRepository cadastroRepository)
        {
            _autenticacaoService = autenticacaoService;
            _notificador = notificador;
            _logger = logger;
            _cadastroRepository = cadastroRepository;
        }

        #region Usuario

        public async Task<bool> InserirUsuario(UsuarioCadastroModel usuario)
        {
            try
            {
                usuario.IdValorPerfil = usuario.IdValorPerfil != 0 ? usuario.IdValorPerfil : EPerfil.Cliente;

                var ultimoIdInserido = await _cadastroRepository.InserirUsuario(usuario);

                if (ultimoIdInserido > 0)
                {
                    var senhaCriptografada =
                        _autenticacaoService.CriptografarSenha(ultimoIdInserido.ToString());

                    var resposta =
                        await _cadastroRepository.CadastrarPrimeiraSenha(usuario.Email, senhaCriptografada);


                    _notificador.Handle(resposta
                        ? new Notificacao("Sucesso", $"{usuario.Nome} cadastrado.", ETipoNotificacao.success.ToString())
                        : new Notificacao("Atenção", $"{usuario.Nome} não foi cadastrado",
                            ETipoNotificacao.warning.ToString()));
                    
                    return resposta;
                }

                _notificador.Handle(new Notificacao("Atenção", $"{usuario.Nome} não foi cadastrado",
                    ETipoNotificacao.warning.ToString()));

                return false;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Duplicate entry"))
                    _notificador.Handle(new Notificacao("Atenção", "Este e-mail já está cadastrado",
                        ETipoNotificacao.warning.ToString()));
                else
                {
                    _notificador.Handle(new Notificacao("Erro", "Erro ao adicionar usuário",
                        ETipoNotificacao.error.ToString()));
                    _logger.LogError($"Erro Inserir usuário, Email: {usuario.Email} - {e.Message}", e);
                }

                return false;
            }
        }

        public async Task<bool> AtualizarUsuario(CadUsuarioModel usuario)
        {
            try
            {
                var resposta = await _cadastroRepository.AtualizarUsuario(usuario);

                _notificador.Handle(resposta
                    ? new Notificacao("Sucesso", $"{usuario.Nome} atualizado.", ETipoNotificacao.success.ToString())
                    : new Notificacao("Atenção", $"{usuario.Nome} não foi atualizado",
                        ETipoNotificacao.warning.ToString()));
                
                return resposta;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao atualizar usuário, Email: {usuario.Email} - {e.Message}", e);
                return false;
            }
        }

        public async Task<bool> AtualizarParcialUsuario(EditarUsuarioModel usuario)
        {
            try
            {
                var resposta = await _cadastroRepository.AtualizarParcialUsuario(usuario);

                _notificador.Handle(resposta
                    ? new Notificacao("Sucesso", $"{usuario.Nome} atualizado.", ETipoNotificacao.success.ToString())
                    : new Notificacao("Atenção", $"{usuario.Nome} não foi atualizado",
                        ETipoNotificacao.warning.ToString()));
                
                return resposta;

            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao atualizar usuário, Email: {usuario.Email} - {e.Message}", e);
                return false;
            }
        }

        public async Task<List<UsuarioViewModel>> BuscarUsuarioList()
        {
            try
            {
                var usuarioList = await _cadastroRepository.BuscarUsuarioList();
                var usuarios = usuarioList.Select(usuario => (UsuarioViewModel)usuario).ToList();
                return usuarios;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao buscar lista de usuários. {e.Message}", e);
                return null;
            }
        }

        public async Task<UsuarioInfoViewModel> BuscarUsuario(int id)
        {
            try
            {
                return await _cadastroRepository.BuscarUsuarioPorId(id);
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao buscar usuário. IdUsuario: {id}. {e.Message}", e);
                return null;
            }
        }

        public async Task<AtualizacaoCadastralViewModel> BuscarDadosParaAtualizacaoCadastral(int id)
        {
            try
            {
                var responta = await _cadastroRepository.BuscarDadosParaAtualizacaoCadastral(id);
                return responta;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao buscar informações do cliente",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao buscar dados para atualizar cadastro. IdUsuario: {id}. {e.Message}", e);
                return null;
            }
        }

        public async Task<bool> AtualizarCadastro(AtualizacaoCadastralViewModel usuario)
        {
            try
            {
                var resposta = await _cadastroRepository.AtualizarCadastro(usuario);

                _notificador.Handle(resposta
                    ? new Notificacao("Sucesso", "Cadastro atualizado.", ETipoNotificacao.success.ToString())
                    : new Notificacao("Atenção", "Cadastro não foi atualizado",
                        ETipoNotificacao.warning.ToString()));
                
                return resposta;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao atualizar cadastro",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao atualizar cadastro. IdUsuario: {usuario.idUsuario}. {e.Message}", e);
                throw;
            }
        }

        public async Task<List<PerfilViewModel>> BuscarPerfilList()
        {
            try
            {
                return await _cadastroRepository.BuscarPerfilList();
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao buscar lista de perfil",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao buscar lista de perfil. {e.Message}", e);
                throw;
            }
        }

        #endregion

        #region Reciclavel

        public async Task<bool> InserirReciclavel(ReciclavelModel reciclavel)
        {
            try
            {
                var retorno = await _cadastroRepository.InserirReciclavel(reciclavel);

                _notificador.Handle(retorno
                    ? new Notificacao("Sucesso", $"{reciclavel.Nome} cadastrado.", ETipoNotificacao.success.ToString())
                    : new Notificacao("Atenção", $"{reciclavel.Nome} não foi cadastrado",
                        ETipoNotificacao.warning.ToString()));
                
                return retorno;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao inserir reciclável. {e.Message}", e);
                return false;
            }
        }

        public async Task<bool> AtualizarReciclavel(ReciclavelModel reciclavel)
        {
            try
            {
                var retorno = await _cadastroRepository.AtualizarReciclavel(reciclavel);
                _notificador.Handle(retorno
                    ? new Notificacao("Sucesso", $"{reciclavel.Nome} atualizado.", ETipoNotificacao.success.ToString())
                    : new Notificacao("Atenção", $"{reciclavel.Nome} não foi atualizado",
                        ETipoNotificacao.warning.ToString()));
                return retorno;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao atualizar reciclavel. {e.Message}", e);
                return false;
            }
        }

        public async Task<bool> ExcluirReciclavel(int id)
        {
            try
            {
                var retorno = await _cadastroRepository.ExcluirReciclavel(id);
                _notificador.Handle(retorno
                    ? new Notificacao("Sucesso", "Item excluido.", ETipoNotificacao.success.ToString())
                    : new Notificacao("Atenção", "Item não foi cadastrado",
                        ETipoNotificacao.warning.ToString()));
                return retorno;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao excluir reciclável. {e.Message}", e);
                return false;
            }
        }

        public async Task<List<ReciclavelViewModel>> BuscarReciclavelList()
        {
            try
            {
                return await _cadastroRepository.BuscarReciclavelList();
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao buscar lista de reciclável. {e.Message}", e);
                return null;
            }
        }

        public async Task<ReciclavelViewModel> BuscarReciclavel(int id)
        {
            try
            {
                return await _cadastroRepository.BuscarReciclavel(id);
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao buscar reciclável. Id: {id}. {e.Message}", e);
                return null;
            }
        }

        #endregion

        #region Endereço

        public async Task<bool> AtualizarEndereco(EnderecoModel endereco)
        {
            try
            {
                var retorno = await _cadastroRepository.AtualizarEndereco(endereco);
                _notificador.Handle(retorno
                    ? new Notificacao("Sucesso", "Endereço atualizado.", ETipoNotificacao.success.ToString())
                    : new Notificacao("Atenção", "Endereço não foi cadastrado",
                        ETipoNotificacao.warning.ToString()));
                return retorno;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao atualizar endereço. {e.Message}", e);
                return false;
            }
        }

        public async Task<List<EnderecoViewModel>> BuscarEnderecoList()
        {
            try
            {
                return await _cadastroRepository.BuscarEnderecoList();
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao buscar lista de endereço. {e.Message}", e);
                return null;
            }
        }

        public async Task<EnderecoViewModel> BuscarEnderecoPorCep(string cep)
        {
            try
            {
                return await _cadastroRepository.BuscarEnderecoPorCep(cep);
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao buscar endereço, cep {cep}. {e.Message}", e);
                return null;
            }
        }

        #endregion

        #region Parâmetros usuário

        public async Task<bool> PrecisaCompletarCadastro(int idUsuario)
        {
            try
            {
                var resposta = await _cadastroRepository.PrecisaCompletarCadastro(idUsuario);
                return resposta;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao verificar primeiro cadastro, idUsuario: {idUsuario}. {e.Message}", e);
                _notificador.Handle(new Notificacao("Erro", "Erro ao verificar primeiro cadastro",
                    ETipoNotificacao.error.ToString()));
                return false;
            }
        }

        public async Task<bool> AtualizarParametroUsuario(ParametroUsuarioModel parametro)
        {
            try
            {
                var resposta = await _cadastroRepository.AtualizarParametroUsuario(parametro);
                return resposta;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao verificar primeiro cadastro, idUsuario: {parametro.IdUsuario}. {e.Message}",
                    e);
                _notificador.Handle(new Notificacao("Erro", "Erro ao atualizar parâmetros",
                    ETipoNotificacao.error.ToString()));
                return false;
            }
        }

        #endregion
    }
}