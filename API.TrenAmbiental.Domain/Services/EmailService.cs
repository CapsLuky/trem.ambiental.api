using System.Net;
using System.Net.Mail;
using API.TrenAmbiental.Domain.Services.Interfaces;

namespace API.TrenAmbiental.Domain.Services
{
    public class EmailService : IEmailService
    {
        public void EnviarEmail(string emailPara, string assunto, string corpoDoEmail, string complemento)
        {
            const string from = "recuperarsenha@tren.eco.br"; // E-mail de remetente cadastrado no painel
            const string user = "recuperarsenha@tren.eco.br"; // Usuário de autenticação do servidor SMTP
            const string pass = "Tren@SenhaEmail10"; // Senha de autenticação do servidor SMTP
            var body = corpoDoEmail + complemento;

            var message = new MailMessage(from, emailPara, assunto, body);
           
            using var smtp = new SmtpClient("email-ssl.com.br", 587);
            smtp.Credentials = new NetworkCredential(user, pass);
            smtp.Send(message);
        }
    }
}
