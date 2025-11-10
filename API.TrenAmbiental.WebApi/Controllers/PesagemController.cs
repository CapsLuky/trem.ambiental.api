using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Model.Pesagem;
using API.TrenAmbiental.DTO.ViewModel.Pesagem;
using API.TrenAmbiental.WebApi.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.TrenAmbiental.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PesagemController : MainController
    {
        private static IPesagemService _pesagemService;

        public PesagemController(
            IPesagemService pesagemService,
            INotificador notificador) : base(notificador)
        {
            _pesagemService = pesagemService;
        }

        // [Authorize(Roles = "1, 2")]
        // [HttpPatch]
        // public async Task<ActionResult<bool>> CreditarConsumidor([FromBody] UsuarioPontoModel credito)
        // {
        //     if (!ModelState.IsValid)
        //         return CustomResponse(ModelState);
        //
        //     var resultado = await _pesagemService.CreditarUsuario(credito);
        //
        //     if (resultado) return CustomResponse();
        //
        //     NotificarErro("Erro", "Erro ao creditar usuário");
        //     return CustomResponse(false);
        // }

        [Authorize(Roles = "1, 2")]
        [HttpGet("pesquisarUsuario/{textoPesquisa}")]
        public async Task<ActionResult<List<PesquisaUsuarioViewModel>>> PesquisarUsuario(string textoPesquisa)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _pesagemService.PesquisarUsuario(textoPesquisa);
            if (resultado is not null)
                return CustomResponse(resultado);

            NotificarErro("Erro", "Tivemos um problema ao pesquisar");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1, 2")]
        [HttpGet("listaReciclavel")]
        public async Task<ActionResult<List<ItemReciclavelViewModel>>> BuscarReciclavelList()
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _pesagemService.BuscarReciclavelList();
            if (resultado is not null)
                return CustomResponse(resultado);

            NotificarErro("Erro", "Não foi possível buscar recicláveis");
            return CustomResponse(false);
        }

        [Authorize(Roles = "1, 2")]
        [HttpPost("HistoricoPesagem")]
        public async Task<ActionResult<bool>> GravarHistoricoPesagem([FromBody] PesagemHistoricoModel pesagemHistorico)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var historicoGravado = await _pesagemService.GravarHistoricoPesagem(pesagemHistorico);

            if (historicoGravado) return CustomResponse(historicoGravado);

            return CustomResponse(false);
        }

        [Authorize(Roles = "1")]
        [HttpPatch("HistoricoPesagemCancelarLancameno")]
        public async Task<ActionResult<bool>> HistoricoPesagemCancelarLancameno([FromBody] int idLancamento)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var historicoGravado = await _pesagemService.HistoricoPesagemCancelarLancameno(idLancamento);

            return historicoGravado ? CustomResponse(historicoGravado) : CustomResponse(false);
        }

        [Authorize(Roles = "1, 2")]
        [HttpGet("HistoricoPesagem")]
        public async Task<ActionResult<List<PesagemHistoricoViewModel>>> BuscarHistoricoPesagemList()
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var result = await _pesagemService.BuscarHistoricoPesagemList();

            return CustomResponse(result);
        }

        [Authorize(Roles = "1, 2, 4")]
        [HttpGet("HistoricoPesagem/{idUsuarioCliente:int}")]
        public async Task<ActionResult<List<PesagemHistoricoViewModel>>> BuscarHistoricoPesagemList(
            int idUsuarioCliente)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var result = await _pesagemService.BuscarHistoricoPesagemList(idUsuarioCliente);

            return CustomResponse(result);
        }

        //[Authorize(Roles = "1, 2, 4")]
        [HttpPost("HistoricoPesagemEntreData")]
        public async Task<ActionResult<List<PesagemHistoricoViewModel>>> BuscarHistoricoPesagemPorDataList(
            [FromBody] PesagemFiltroHistoricoModel filtro)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var validado = _pesagemService.ValidarAlgumaCoisa();

            if (!validado) return CustomResponse(string.Empty);

            var result = await _pesagemService.BuscarHistoricoPesagemPorDataList(filtro);

            return CustomResponse(result);
        }
        
        [Authorize(Roles = "1, 4")]
        [HttpPost("Peso")]
        public async Task<ActionResult<int>> BuscarPontuacaoUsuario([FromBody] PesoFiltroModel filtro)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _pesagemService.BuscarPesoUsuario(filtro);
            return CustomResponse(resultado != -1 ? resultado : 0);
        }
        
    }
}