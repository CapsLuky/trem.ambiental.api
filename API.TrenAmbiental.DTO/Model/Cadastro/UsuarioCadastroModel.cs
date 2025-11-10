using API.TrenAmbiental.DTO.Enum;

namespace API.TrenAmbiental.DTO.Model.Cadastro
{
    public class UsuarioCadastroModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public EPerfil IdValorPerfil { get; set; }
        public bool Ativo { get; set; }
    }
}