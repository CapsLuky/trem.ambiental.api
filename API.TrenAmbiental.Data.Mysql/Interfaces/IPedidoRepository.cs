using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.DTO.Entidade.Venda;
using API.TrenAmbiental.DTO.Enum.Venda;
using API.TrenAmbiental.DTO.ViewModel.Carrinho;

namespace API.TrenAmbiental.Data.Mysql.Interfaces
{
    public interface IPedidoRepository
    {
        Task<IEnumerable<Pedido>> ObterListaPorClienteId(int idUsuario);
        Task<Pedido> ObterPedidoCliente(int idUsuario, EPedidoStatus pedidoStatus);
        Task<Pedido> ObterPedidoPorId(int idPedido);
        bool AdicionarPedido(Pedido pedido);
        bool AtualizarPedido(Pedido pedido);
        bool RemoverPedido(int id);
        Task<PedidoItem> ObterItemPorId(int id);
        Task<PedidoItem> ObterItemPorPedido(int idPedido, int idProduto);
        bool AdicionarItem(PedidoItem pedidoItem);
        bool AtualizarItem(PedidoItem pedidoItem);
        bool RemoverItem(PedidoItem pedidoItem);
        Task<List<int>> ObterPedidosExpirados(string dataAtual);
        Task<List<Pedido>> ObterPedidoPorIdUsuario(int idUsuario);
        Task<List<PedidoViewModel>> ObterTodosOsPedidos();
        Task<List<PedidoStatusViewModel>> ObterPedidoStatus();
    }
}