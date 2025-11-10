using System;

namespace API.TrenAmbiental.DTO.Model.Pesagem
{
    public class PesoFiltroModel
    {
        public int idUsuario { get; set; }
        public DateTime? dataInicio { get; set; }
        public DateTime? dataFim { get; set; }
    }
}