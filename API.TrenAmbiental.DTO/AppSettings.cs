namespace API.TrenAmbiental.DTO
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int ExpiracaoHoras { get; set; }
        public string Emissor { get; set; }
        public string ValidoEm { get; set; }
        public string CaminhoFotos { get; set; }
        public string EmailEstoqueBaixo { get; set; }
    }
}
