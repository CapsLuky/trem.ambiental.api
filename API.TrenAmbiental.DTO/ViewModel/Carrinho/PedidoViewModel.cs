using System;

namespace API.TrenAmbiental.DTO.ViewModel.Carrinho
{
    public class PedidoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public int ValorTotal { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime Validade { get; set; }
        public DateTime? DataPedido { get; set; }
        public DateTime? DataFechamento { get; set; }
    }
}