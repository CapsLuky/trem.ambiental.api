using System.Collections.Generic;
using System.Net;

namespace API.TrenAmbiental.DTO.DomainObjects
{
    public interface INotificador
    {
        bool TemNotificacao(string tipo);
        List<Notificacao> ObterNotificacoes();
        HttpStatusCode ObterStatusCode();
        void Handle(Notificacao notificacao, HttpStatusCode statusCode = HttpStatusCode.OK);
        void ApagarNotificacoes();
    }
}