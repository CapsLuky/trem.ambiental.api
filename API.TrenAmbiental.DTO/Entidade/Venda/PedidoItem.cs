using System;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.ViewModel.Catalogo;

namespace API.TrenAmbiental.DTO.Entidade.Venda
{
    public class PedidoItem
    {
        public int Id { get; }
        public int IdPedido { get; private set; }
        public int IdProduto { get; private set; }
        public string ProdutoNome { get; private set; }
        public int Quantidade { get; private set; }
        public int ValorUnitario { get; private set; }
        public DateTime Validade { get; private set; }

        protected PedidoItem()
        {
        }

        public PedidoItem(int id, int idPedido, int idProduto, string produtoNome, int quantidade, int valorUnitario,
            DateTime validade)
        {
            Id = id;
            IdPedido = idPedido;
            IdProduto = idProduto;
            ProdutoNome = produtoNome;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            Validade = validade;
        }

        internal void AssociarPedido(int idPedido)
        {
            IdPedido = idPedido;
        }

        public int CalcularValor()
        {
            return Quantidade * ValorUnitario;
        }

        public void AdicionarUnidades(int unidades)
        {
            Quantidade += unidades;
        }

        internal void AtualizarUnidades(int unidades)
        {
            Quantidade = unidades;
        }

        internal static DateTime CalcularDataVencimento(short dias)
        {
            return DataHoraAtual.ObterDataHoraServidorWindows().AddDays(dias);
        }

        public static implicit operator PedidoItem(ProdutoViewModel produtoViewModel)
        {
            return new PedidoItem
            {
                IdProduto = produtoViewModel.Id,
                ProdutoNome = produtoViewModel.Nome,
                ValorUnitario = produtoViewModel.ValorPontos,
                Validade = CalcularDataVencimento(produtoViewModel.Validade)
            };
        }
    }
}