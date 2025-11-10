namespace API.TrenAmbiental.DTO.ViewModel.Cadastro
{
    public class ReciclavelViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Ponto { get; set; }
        public decimal EficienciaEnergetica { get; set; }
        public bool Ativo { get; set; }
    }
}
