using API.TrenAmbiental.DTO.DomainObjects;

namespace API.TrenAmbiental.DTO.Entidade.Carrinho
{
    public class IniciarPedido
    {
        public int IdPedido { get; }
        public int IdUsuario { get; }

        public int Total { get; }
        // public string NomeCartao { get; private set; }
        // public string NumeroCartao { get; private set; }
        // public string ExpiracaoCartao { get; private set; }
        // public string CvvCartao { get; private set; }

        public IniciarPedido(int idPedido, int idUsuario, int total)
        {
            IdPedido = idPedido;
            IdUsuario = idUsuario;
            Total = total;
            // NomeCartao = nomeCartao;
            // NumeroCartao = numeroCartao;
            // ExpiracaoCartao = expiracaoCartao;
            // CvvCartao = cvvCartao;
            Validar();
        }

        public void Validar()
        {
            Validacoes.ValidarSeMenorQue(Total, 1, "Valor inválido");
        }
    }
}