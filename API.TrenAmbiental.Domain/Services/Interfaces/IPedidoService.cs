using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.DTO.Model.Carrinho;
using API.TrenAmbiental.DTO.Model.Catalogo;
using API.TrenAmbiental.DTO.ViewModel.Carrinho;

namespace API.TrenAmbiental.Domain.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<bool> EnviarItemPedidoParaCarrinho(PedidoItemModel pedidoItemModel);
        Task<bool> AdicionarItemPedido(PedidoItemModel pedidoItemModel);
        Task<CarrinhoViewModel> ObterCarrinhoCliente(int idUsuario);
        Task<IEnumerable<SituacaoPedidoViewModel>> ObterPedidoPorIdUsuario(int idUsuario);
        Task<SituacaoPedidoViewModel> ObterSituacaoPedido(int idPedido);
        Task<bool> AtualizarItemPedido(AtualizarItemPedidoModel atualizarItemPedidoModel);
        Task<bool> RemoverItemPedido(RemoverItemPedidoModel removerItemPedidoModel);
        Task<bool> IniciarPedido(int idUsuario);
        Task<bool> FinalizarPedido(FinalizarPedidoModel finalizarPedidoModel);
        Task<int> ExpirarPedidosForaDaValidade();
        Task<List<PedidoViewModel>> ObterTodosOsPedidos();
        Task<List<PedidoStatusViewModel>> ObterPedidoStatus();
    }
}