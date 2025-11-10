namespace API.TrenAmbiental.Domain.Services.Interfaces
{
    public interface IEmailService
    {
        void EnviarEmail(string emailPara, string assunto, string corpoDoEmail, string complemento);
    }
}