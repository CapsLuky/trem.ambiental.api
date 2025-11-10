namespace API.TrenAmbiental.DTO.Model.Catalogo;

public class RemoverItemPedidoModel
{
    public int IdUsuario { get; }
    public int IdProduto { get; }

    public RemoverItemPedidoModel(int idUsuario, int idProduto)
    {
        IdUsuario = idUsuario;
        IdProduto = idProduto;
    }
}