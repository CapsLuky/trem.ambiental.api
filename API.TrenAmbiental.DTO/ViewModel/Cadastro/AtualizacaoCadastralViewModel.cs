namespace API.TrenAmbiental.DTO.ViewModel.Cadastro
{
    public class AtualizacaoCadastralViewModel
    {
        public int idUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Apelido { get; set; }
        public int Idade { get; set; }
        public string Genero { get; set; }
        public int IdGenero { get; set; }
        public string Cep { get; set; }
        public string Condominio { get; set; }
    }
}