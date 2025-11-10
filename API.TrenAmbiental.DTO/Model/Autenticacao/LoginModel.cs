using System.ComponentModel.DataAnnotations;

namespace API.TrenAmbiental.DTO.Model.Autenticacao
{
    public class LoginModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Senha { get; set; }
    }
}