namespace API.TrenAmbiental.DTO.Model.Carrinho;

public class AtualizarItemPedidoModel
{
    public int IdUsuario { get; }
    public int IdProduto { get; }
    public int Quantidade { get; }

    public AtualizarItemPedidoModel(int idUsuario, int idProduto, int quantidade)
    {
        IdUsuario = idUsuario;
        IdProduto = idProduto;
        Quantidade = quantidade;

        Validar();
    }

    public void Validar()
    {
        //Validacoes.ValidarSeMenorQue(Quantidade, 1, "A quantidade miníma de um item é 1");
        //Validacoes.ValidarSeMaiorQue(Quantidade, 99, "A quantidade máxima de um item é 99");
    }
}