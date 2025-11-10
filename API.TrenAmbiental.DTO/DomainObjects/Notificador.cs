using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace API.TrenAmbiental.DTO.DomainObjects
{
    public class Notificador : INotificador
    {
        private readonly List<Notificacao> _notificacoes;
        private HttpStatusCode _statusCode;

        public Notificador()
        {
            _notificacoes = new List<Notificacao>();
        }

        public void Handle(Notificacao notificacao, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _notificacoes.Add(notificacao);
            _statusCode = statusCode;
        }

        public void ApagarNotificacoes()
        {
            _notificacoes.Clear();
        }

        public List<Notificacao> ObterNotificacoes()
        {
            return _notificacoes;
        }

        public HttpStatusCode ObterStatusCode()
        {
            return _statusCode;
        }

        public bool TemNotificacao(string tipo)
        {
            return _notificacoes.Any(notificacao => notificacao.Tipo == tipo);
        }
    }
}