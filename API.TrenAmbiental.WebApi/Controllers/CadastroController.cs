using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Model;
using API.TrenAmbiental.DTO.Model.Cadastro;
using API.TrenAmbiental.DTO.ViewModel.Cadastro;
using API.TrenAmbiental.WebApi.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.TrenAmbiental.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CadastroController : MainController
    {
        private static ICadastroService _cadastroService;

        public CadastroController(ICadastroService cadastroService,
            INotificador notificador) : base(notificador)
        {
            _cadastroService = cadastroService;
        }

        #region Usuario

        [Authorize(Roles = "1, 2")]
        [HttpPost("usuario")]
        public async Task<ActionResult<bool>> InserirUsuario([FromBody] UsuarioCadastroModel usuario)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.InserirUsuario(usuario);

            if (resultado) return CustomResponse();

            return CustomResponse(false);
        }

        [Authorize(Roles = "1, 2")]
        [HttpPut("usuario")]
        public async Task<ActionResult<bool>> AtualizarUsuario([FromBody] CadUsuarioModel usuario)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.AtualizarUsuario(usuario);

            if (resultado) return CustomResponse();

            NotificarErro("Erro", "Não foi possível atualizar as informações");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1, 2")]
        [HttpPatch("usuario")]
        public async Task<ActionResult<bool>> AtualizarParcialUsuario([FromBody] EditarUsuarioModel usuario)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.AtualizarParcialUsuario(usuario);
            if (resultado) return CustomResponse();

            NotificarErro("Erro", "Não foi possível atualizar as informações");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1, 2")]
        [HttpGet("usuario")]
        public async Task<ActionResult<List<UsuarioInfoViewModel>>> BuscarUsuarioList()
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.BuscarUsuarioList();

            if (resultado is not null) return CustomResponse(resultado);
            NotificarErro("Erro", "Não foi possível buscar a lista de usuários");
            return CustomResponse(false);

        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet("usuario/{id:int}")]
        public async Task<ActionResult<UsuarioInfoViewModel>> BuscarUsuario(int id)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.BuscarUsuario(id);

            if (resultado is not null) return CustomResponse(resultado);
            NotificarErro("Erro", "Não foi buscar usuário");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet("usuario/BuscarDadosAtualizacaoCadastro/{idUsuario:int}")]
        public async Task<ActionResult<AtualizacaoCadastralViewModel>> BuscarDadosAtualizacaoCadastro(int idUsuario)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _cadastroService.BuscarDadosParaAtualizacaoCadastral(idUsuario);
            return CustomResponse(resultado);
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPut("usuario/atualizarCadastro")]
        public async Task<ActionResult<bool>> AtualizarCadastro([FromBody] AtualizacaoCadastralViewModel usuario)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _cadastroService.AtualizarCadastro(usuario);

            if (resultado) return CustomResponse(true);

            NotificarErro("Erro", "Não foi possível atualizar as informações");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1")]
        [HttpGet("perfil")]
        public async Task<ActionResult<List<PerfilViewModel>>> BuscarPerfil()
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _cadastroService.BuscarPerfilList();

            return CustomResponse(resultado);
        }

        #endregion

        #region Reciclavel

        [Authorize(Roles = "1")]
        [HttpPost("reciclavel")]
        public async Task<ActionResult<bool>> InserirReciclavel([FromBody] ReciclavelModel reciclavel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.InserirReciclavel(reciclavel);
            if (resultado)
                return CustomResponse();

            NotificarErro("Erro", "Não foi possível inserir informações");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1")]
        [HttpPut("reciclavel")]
        public async Task<ActionResult<bool>> AtualizarReciclavel([FromBody] ReciclavelModel reciclavel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.AtualizarReciclavel(reciclavel);
            if (resultado)
                return CustomResponse();

            NotificarErro("Erro", "Não foi possível atualizar as informações");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1")]
        [HttpDelete("reciclavel/{id:int}")]
        public async Task<ActionResult<bool>> ExcluirReciclavel(int id)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.ExcluirReciclavel(id);
            if (resultado)
                return CustomResponse();

            NotificarErro("Erro", "Não foi possível excluir");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1")]
        [HttpGet("reciclavel")]
        public async Task<ActionResult<List<ReciclavelViewModel>>> BuscarReciclavelList()
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.BuscarReciclavelList();
            if (resultado is not null)
                return CustomResponse(resultado);

            NotificarErro("Erro", "Não foi possível buscar recicláveis");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1")]
        [HttpGet("reciclavel/{id:int}")]
        public async Task<ActionResult<ReciclavelViewModel>> BuscarReciclave(int id)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.BuscarReciclavel(id);
            if (resultado is not null)
                return CustomResponse(resultado);

            NotificarErro("Erro", "Não foi possível buscar reciclável");
            return CustomResponse(false);
        }

        #endregion

        #region Endereço

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPatch("endereco")]
        public async Task<ActionResult<bool>> AtualizarEndereco([FromBody] EnderecoModel endereco)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.AtualizarEndereco(endereco);
            if (resultado)
                return CustomResponse();

            NotificarErro("Erro", "Não foi possível atualizar.");
            return CustomResponse(false);
            ;
        }

        [Authorize(Roles = "1, 2")]
        [HttpGet("endereco")]
        public async Task<ActionResult<List<EnderecoViewModel>>> BuscarEnderecoList()
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.BuscarEnderecoList();
            if (resultado is not null)
                return CustomResponse(resultado);

            NotificarErro("Erro", "Não foi buscar endereços");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet("endereco/{cep:length(8)}")]
        public async Task<ActionResult<EnderecoViewModel>> BuscarEnderecoPorCep(string cep)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);
            
            var resultado = await _cadastroService.BuscarEnderecoPorCep(cep);
            if (resultado is not null)
                return CustomResponse(resultado);

            NotificarErro("Erro", "Não foi buscar endereço");
            return CustomResponse(false);
        }

        #endregion

        #region Parâmetros usuário

        /// <response code="200">Verifica se o é preciso completar o cadastro no primeiro acesso</response>
        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet("usuario/completarCadastro/{idUsuario:int}")]
        public async Task<ActionResult<bool>> PrecisaCompletarCadastro(int idUsuario)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _cadastroService.PrecisaCompletarCadastro(idUsuario);
            return CustomResponse(Convert.ToBoolean(resultado));
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPatch("usuario/atualizarParametro")]
        public async Task<ActionResult<bool>> AtualizarParametroUsuario([FromBody] ParametroUsuarioModel parametro)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _cadastroService.AtualizarParametroUsuario(parametro);

            if (resultado) return CustomResponse();

            NotificarErro("Erro", "Erro ao atualizar parâmetros");
            return CustomResponse(false);
        }

        #endregion
    }
}