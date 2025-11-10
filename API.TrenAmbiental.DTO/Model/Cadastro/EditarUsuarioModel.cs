using API.TrenAmbiental.DTO.Entidade;

namespace API.TrenAmbiental.DTO.Model.Cadastro
{
    public class EditarUsuarioModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public Perfil Perfil { get; set; }
        public short IdValorPerfil { get; set; }
        public bool Ativo { get; set; }
    }
}