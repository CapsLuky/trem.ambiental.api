using API.TrenAmbiental.DTO.Enum.Venda;

namespace API.TrenAmbiental.DTO.Model.Carrinho;

public class FinalizarPedidoModel
{
    public int IdPedido { get; set; }
    public EPedidoStatus PedidoStatus { get; set; }
}