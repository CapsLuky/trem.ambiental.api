namespace API.TrenAmbiental.DTO.ViewModel.Carrinho;

public class CarrinhoItemViewModel
{
    public int IdProduto { get; set; }
    public string ProdutoNome { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
}