using System;
using System.Collections.Generic;

namespace API.TrenAmbiental.DTO.ViewModel.Carrinho
{
    public class CarrinhoViewModel
    {
        public int IdPedido { get; set; }
        public int IdUsuario { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime Validade { get; set; }

        public List<CarrinhoItemViewModel> Items { get; set; } = new();

        // INFORMAÇÕES DE PAGAMENTO
        //public CarrinhoPagamentoViewModel Pagamento { get; set; }
    }
}