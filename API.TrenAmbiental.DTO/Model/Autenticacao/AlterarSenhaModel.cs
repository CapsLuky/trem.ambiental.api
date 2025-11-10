namespace API.TrenAmbiental.DTO.Model.Autenticacao
{
    public class AlterarSenhaModel
    {
        public string Email { get; set; }
        public string SenhaAtual { get; set; }
        public string SenhaNova { get; set; }
        public string TokenRedefinirSenha { get; set; }
    }
}
