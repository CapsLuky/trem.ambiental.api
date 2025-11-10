using System;
using System.Collections.Generic;
using API.TrenAmbiental.DTO.Enum.Venda;

namespace API.TrenAmbiental.DTO.ViewModel.Carrinho;

public class SituacaoPedidoViewModel
{
    public int IdPedido { get; set; }
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; }
    public decimal ValorTotal { get; set; }
    public EPedidoStatus PedidoStatus { get; set; }
    public DateTime Validade { get; set; }
    public DateTime? DataPedido { get; set; }

    public List<CarrinhoItemViewModel> Items { get; set; } = new();
}