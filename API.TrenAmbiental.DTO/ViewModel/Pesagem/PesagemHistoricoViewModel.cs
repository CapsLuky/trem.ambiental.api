using System;

namespace API.TrenAmbiental.DTO.ViewModel.Pesagem
{
    public class PesagemHistoricoViewModel
    {
        public int Id { get; set; }
        public string Operador { get; set; }
        public string Cliente { get; set; }
        public string Reciclavel { get; set; }
        public decimal Peso { get; set; }
        public int Ponto { get; set; }
        public DateTime DataLancamento { get; set; }
        public bool Cancelado { get; set; }
        public DateTime DataCancelado { get; set; }
    }
}