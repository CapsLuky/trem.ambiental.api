namespace API.TrenAmbiental.DTO.Model.Cadastro
{
    public class CadUsuarioModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public string Email { get; set; }
        public short Idade { get; set; }
        public short IdGenero { get; set; }
        public short IdValorPerfil { get; set; }
        public string Senha { get; set; }
    }
}