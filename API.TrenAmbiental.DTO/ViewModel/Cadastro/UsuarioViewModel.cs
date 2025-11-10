using API.TrenAmbiental.DTO.Entidade;
using API.TrenAmbiental.DTO.Entidade.Cadastro;

namespace API.TrenAmbiental.DTO.ViewModel.Cadastro
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public Perfil Perfil { get; set; }
        public bool Ativo { get; set; }

        public static implicit operator UsuarioViewModel(Usuario usuario)
        {
            return new()
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = new Perfil
                {
                    NomePerfil = usuario.NomePerfil,
                    Valor = usuario.IdValorPerfil
                },
                Ativo = usuario.Ativo
            };
        }
    }
}