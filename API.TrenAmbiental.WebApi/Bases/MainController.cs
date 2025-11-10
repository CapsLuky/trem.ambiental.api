using System.Linq;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.TrenAmbiental.WebApi.Bases
{
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly INotificador _notificador;

        public MainController(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao(ETipoNotificacao.error.ToString());
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
                return Ok(new
                {
                    success = true,
                    payload = result,
                    messages = _notificador.ObterNotificacoes()
                });

            return OperacaoInvalida();
        }

        protected ActionResult OperacaoInvalida()
        {
            var result = new
            {
                success = false,
                erro = _notificador.ObterNotificacoes()
            };

            return new ObjectResult(result)
            {
                StatusCode = (int)_notificador.ObterStatusCode()
            };
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotificarErroModelInvalida(modelState);
            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modalState)
        {
            var erros = ModelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                var errorMgs = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro("Erro", errorMgs);
            }
        }

        protected void NotificarErro(string titulo, string mensagem)
        {
            _notificador.Handle(new Notificacao(titulo, mensagem, ETipoNotificacao.error.ToString()));
        }
    }
}