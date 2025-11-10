namespace API.TrenAmbiental.DTO.Entidade.Cadastro
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public short IdValorPerfil { get; set; }
        public string NomePerfil { get; set; }
        public bool Ativo { get; set; }
    }
}