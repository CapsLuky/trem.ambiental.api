using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Entidade.Pontuacao;
using API.TrenAmbiental.DTO.Model.Pesagem;
using API.TrenAmbiental.WebApi.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.TrenAmbiental.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PontuacaoController : MainController
    {
        private readonly IPontuacaoService _pontuacaoService;

        public PontuacaoController(IPontuacaoService pontuacaoService,
                                   INotificador notificador) : base(notificador)
        {
            _pontuacaoService = pontuacaoService;
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPost]
        public async Task<ActionResult<int>> BuscarPontuacaoUsuario([FromBody] PontuacaoFiltroModel filtro)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _pontuacaoService.BuscarPontuacaoUsuario(filtro);
            return CustomResponse(resultado != -1 ? resultado : 0);
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet("Saldo")]
        public async Task<ActionResult<int>> BuscarSaldoUsuario(int idUsuario)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await _pontuacaoService.BuscarSaldoUsuario(idUsuario);
            return CustomResponse(resultado);
        }

        // [Authorize(Roles = "1, 2, 3, 4")]
        // [HttpGet("BuscarRankingPorMes")]
        // public async Task<ActionResult<List<Ranking>>> BuscarRankingPorMes(DateTime data)
        // {
        //     if (!ModelState.IsValid)
        //         return CustomResponse(ModelState);
        //
        //     return CustomResponse(await _pontuacaoService.BuscarRankingPorMes(data));
        // }
        
        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet("BuscarRanking")]
        public async Task<ActionResult<List<Ranking>>> BuscarRanking()
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            return CustomResponse(await _pontuacaoService.BuscarRanking());
        }

        // [Authorize(Roles = "1, 2, 3, 4")]
        // [HttpGet("BuscarRankingPorIdUsuarioData")]
        // public async Task<ActionResult<Ranking>> BuscarRankingPorIdUsuarioData(int idUsuario, DateTime data)
        // {
        //     if (!ModelState.IsValid)
        //         return CustomResponse(ModelState);
        //
        //     return CustomResponse(await _pontuacaoService.BuscarRankingPorIdUsuarioData(idUsuario, data));
        // }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet("BuscarPosicaoRanking")]
        public async Task<ActionResult<int>> BuscarPosicaoRanking(int idUsuario, bool mesAnterior)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            return CustomResponse(await _pontuacaoService.BuscarPosicaoRanking(idUsuario, mesAnterior));
        }
    }
}