using System;
using API.TrenAmbiental.DTO.DomainObjects;

namespace API.TrenAmbiental.DTO.Model.Pesagem
{
    public class PesagemHistoricoModel
    {
        public int Id { get; set; }
        public int IdUsuarioOperador { get; set; }
        public int IdUsuarioCliente { get; set; }
        public int IdReciclavel { get; set; }
        public decimal Peso { get; set; }
        public int Ponto { get; set; }
        public DateTime DataLancamento { get; private set; }
        public bool Cancelado { get; set; }
        public DateTime DataCancelado { get; private set; }

        public void SetarDataDeLancamento()
        {
            var data = DataHoraAtual.ObterDataHoraServidorWindows();
            DataLancamento = data;
        }

        public void SetarDataDeCancelamento()
        {
            var data = DataHoraAtual.ObterDataHoraServidorWindows();
            DataCancelado = data;
        }
    }
}