using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Model.Carrinho;
using API.TrenAmbiental.DTO.Model.Catalogo;
using API.TrenAmbiental.DTO.ViewModel.Carrinho;
using API.TrenAmbiental.WebApi.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.TrenAmbiental.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CarrinhoController : MainController
    {
        private static IPedidoService _pedidoService;

        public CarrinhoController(
            INotificador notificador,
            IPedidoService pedidoService) : base(notificador)
        {
            _pedidoService = pedidoService;
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet]
        [Route("meuCarrinho/{idUsuario:int}")]
        public async Task<ActionResult<CarrinhoViewModel>> ObterCarrinhoCliente(int idUsuario)
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.ObterCarrinhoCliente(idUsuario));
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet]
        [Route("historicoDePedido/{idUsuario:int}")]
        public async Task<ActionResult<SituacaoPedidoViewModel>> ObterHistoricoPedidosCliente(int idUsuario)
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.ObterPedidoPorIdUsuario(idUsuario));
        }

        [Authorize(Roles = "1, 2, 4")]
        [HttpGet]
        [Route("situacaoPedido/{idPedido:int}")]
        public async Task<ActionResult<SituacaoPedidoViewModel>> ObterSituacaoPedido(int idPedido)
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.ObterSituacaoPedido(idPedido));
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPost]
        [Route("enviarItemParaCarrinho")]
        public async Task<ActionResult<bool>> EnviarItemParaCarrinho(
            [FromBody] PedidoItemModel pedidoItem)
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.EnviarItemPedidoParaCarrinho(pedidoItem));
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPost]
        [Route("AdicionarItem")]
        public async Task<ActionResult<bool>> AdicionarItem([FromBody] PedidoItemModel pedidoItem)
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.AdicionarItemPedido(pedidoItem));
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPost]
        [Route("AtualizarItem")]
        public async Task<ActionResult<bool>> AtualizarItem([FromBody] AtualizarItemPedidoModel pedidoItem)
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.AtualizarItemPedido(pedidoItem));
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPost]
        [Route("removerItem")]
        public async Task<ActionResult<bool>> RemoverItem([FromBody] RemoverItemPedidoModel removerItemPedidoModel)
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.RemoverItemPedido(removerItemPedidoModel));
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPost]
        [Route("iniciarPedido")]
        public async Task<ActionResult<bool>> IniciarPedido(int idUsuario)
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.IniciarPedido(idUsuario));
           
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpPost]
        [Route("finalizarPedido")]
        public async Task<ActionResult<bool>> FinalizarPedido([FromBody] FinalizarPedidoModel finalizarPedidoModel)
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.FinalizarPedido(finalizarPedidoModel));
        }

        [Authorize(Roles = "1, 2, 4")]
        [HttpPost]
        [Route("expirarPedidosForaDaValidade")]
        public async Task<ActionResult<int>> ExpirarPedidos()
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.ExpirarPedidosForaDaValidade());
        }

        [Authorize(Roles = "1, 2, 4")]
        [HttpGet]
        [Route("obterTodosOsPedidos")]
        public async Task<ActionResult<List<PedidoViewModel>>> ObterTodosOsPedidos()
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.ObterTodosOsPedidos());
        }

        [Authorize(Roles = "1, 2, 3, 4")]
        [HttpGet]
        [Route("statusPedido")]
        public async Task<ActionResult<List<PedidoStatusViewModel>>> ObterStatusPedido()
        {
            return !ModelState.IsValid
                ? CustomResponse(ModelState)
                : CustomResponse(await _pedidoService.ObterPedidoStatus());
        }
    }
}