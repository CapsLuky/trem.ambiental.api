using System.Collections.Generic;

namespace API.TrenAmbiental.DTO.Entidade.Carrinho
{
    public class ListaProdutosPedido
    {
        public int IdPedido { get; set; }
        public ICollection<Item> Itens { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
    }
}