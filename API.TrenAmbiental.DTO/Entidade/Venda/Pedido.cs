using System;
using System.Collections.Generic;
using System.Linq;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Enum.Venda;

namespace API.TrenAmbiental.DTO.Entidade.Venda
{
    public class Pedido
    {
        public int Id { get; private set; }
        public int IdUsuario { get; private set; }
        public int ValorTotal { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public EPedidoStatus PedidoStatus { get; private set; }
        public DateTime Validade { get; private set; }
        public DateTime? DataPedido { get; private set; }
        public DateTime? DataFechamento { get; private set; }
        public DateTime? DataUltimaAtualizacao { get; private set; }
        
        
        private readonly List<PedidoItem> _pedidoItems;
        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidoItems;

        protected Pedido()
        {
            _pedidoItems = new List<PedidoItem>();
        }

        public Pedido(int idUsuario, int valorTotal)
        {
            IdUsuario = idUsuario;
            ValorTotal = valorTotal;
            _pedidoItems = new List<PedidoItem>();
        }

        private void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(p => p.CalcularValor());
            DataUltimaAtualizacao = DataHoraAtual.ObterDataHoraServidorWindows();
        }

        public bool PedidoItemExistente(int idProduto)
        {
            return _pedidoItems.Any(p => p.IdProduto == idProduto);
        }

        public void AdicionarItem(PedidoItem item)
        {
            item.AssociarPedido(Id);

            if (PedidoItemExistente(item.IdProduto))
            {
                var itemExistente = _pedidoItems.FirstOrDefault(p => p.IdProduto == item.IdProduto);
                itemExistente.AdicionarUnidades(item.Quantidade);
                item = itemExistente;

                _pedidoItems.Remove(itemExistente);
            }

            item.CalcularValor();
            _pedidoItems.Add(item);

            CalcularValorPedido();
        }

        public void RemoverItem(PedidoItem item)
        {
            var itemExistente = PedidoItems.FirstOrDefault(p => p.IdProduto == item.IdProduto);

            if (itemExistente == null) throw new DomainException("O item não pertence ao pedido");
            _pedidoItems.Remove(itemExistente);

            CalcularValorPedido();
        }

        public void CalcularValidadePedido()
        {
            if (PedidoItems.Count == 0) return;
            var itemComMenorValidade = PedidoItems.OrderBy(p => p.Validade);
            Validade = itemComMenorValidade.First().Validade;
        }

        public void AtualizarItem(PedidoItem item)
        {
            item.AssociarPedido(Id);

            var itemExistente = PedidoItems.FirstOrDefault(p => p.IdProduto == item.IdProduto);

            if (itemExistente == null) throw new DomainException("O item não pertence ao pedido");

            _pedidoItems.Remove(itemExistente);
            _pedidoItems.Add(item);

            CalcularValorPedido();
        }

        public void AtualizarUnidades(PedidoItem item, int unidades)
        {
            item.AtualizarUnidades(unidades);
            AtualizarItem(item);
        }

        public void TornarRascunho()
        {
            PedidoStatus = EPedidoStatus.Rascunho;
            DataUltimaAtualizacao = DataHoraAtual.ObterDataHoraServidorWindows();
        }

        public void IniciarPedido()
        {
            PedidoStatus = EPedidoStatus.EmAberto;
            DataPedido = DataHoraAtual.ObterDataHoraServidorWindows();
            DataUltimaAtualizacao = DataHoraAtual.ObterDataHoraServidorWindows();
        }

        private void ExpirarPedido()
        {
            PedidoStatus = EPedidoStatus.Expirado;
            DataFechamento = DataHoraAtual.ObterDataHoraServidorWindows();
            DataUltimaAtualizacao = DataHoraAtual.ObterDataHoraServidorWindows();
        }

        private void EntregarPedido()
        {
            PedidoStatus = EPedidoStatus.Entregue;
            DataFechamento = DataHoraAtual.ObterDataHoraServidorWindows();
            DataUltimaAtualizacao = DataHoraAtual.ObterDataHoraServidorWindows();
        }

        private void CancelarPedido()
        {
            PedidoStatus = EPedidoStatus.Cancelado;
            DataFechamento = DataHoraAtual.ObterDataHoraServidorWindows();
            DataUltimaAtualizacao = DataHoraAtual.ObterDataHoraServidorWindows();
        }

        public void MudarStatusDoPedido(EPedidoStatus pedidoStatus)
        {
            switch (pedidoStatus)
            {
                case EPedidoStatus.Expirado:
                    ExpirarPedido();
                    break;
                case EPedidoStatus.Cancelado:
                    CancelarPedido();
                    break;
                case EPedidoStatus.Entregue:
                    EntregarPedido();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pedidoStatus), pedidoStatus, null);
            }
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(int idUsuario)
            {
                var pedido = new Pedido
                {
                    IdUsuario = idUsuario,
                    DataCadastro = DataHoraAtual.ObterDataHoraServidorWindows(),
                    DataUltimaAtualizacao = DataHoraAtual.ObterDataHoraServidorWindows()
                };

                pedido.TornarRascunho();
                return pedido;
            }
        }
    }
}