namespace API.TrenAmbiental.DTO.Model.Catalogo
{
    public class ProdutoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public int ValorPontos { get; set; }
        public short QuantidadeEstoque { get; set; }
        public short QuantidadeEstoqueBaixo { get; set; }
        public short Validade { get; set; }
        public short Tipo { get; set; }
    }
}
