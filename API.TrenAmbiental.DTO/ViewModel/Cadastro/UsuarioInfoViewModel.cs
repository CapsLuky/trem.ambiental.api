namespace API.TrenAmbiental.DTO.ViewModel.Cadastro
{
    public class UsuarioInfoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public string Email { get; set; }
        public short Idade { get; set; }
        public string Genero { get; set; }
        public short IdGenero { get; set; }
        public string Perfil { get; set; }
        public short IdValorPerfil { get; set; }
        public string AccessToken { get; set; }
        public double ExpiresIn { get; set; }
    }
}