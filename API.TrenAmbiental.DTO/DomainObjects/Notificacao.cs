namespace API.TrenAmbiental.DTO.DomainObjects
{
    public class Notificacao
    {
        public string Titulo { get; }
        public string Mensagem { get; }
        public string Tipo { get; }

        public Notificacao(string titulo, string mensagem, string tipo)
        {
            Tipo = tipo;
            Titulo = titulo;
            Mensagem = mensagem;
        }
    }
}