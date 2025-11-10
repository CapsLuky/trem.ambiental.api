using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.DTO.Entidade.Venda;
using API.TrenAmbiental.DTO.Enum.Venda;
using API.TrenAmbiental.DTO.ViewModel.Carrinho;
using Microsoft.Extensions.Configuration;

namespace API.TrenAmbiental.Data.Mysql.Repositories
{
    public class PedidoRepository : BaseRepository, IPedidoRepository
    {
        public PedidoRepository(IConfiguration config) : base(config)
        {
        }
        public async Task<IEnumerable<Pedido>> ObterListaPorClienteId(int idUsuario)
        {
            const string sql = @"SELECT Id, IdUsuario, ValorTotal, DataCadastro, PedidoStatus
                                 FROM Pedido WHERE IdUsuario = @idUsuario";

            return await QueryAsync<Pedido>(sql, new { idUsuario });
        }

        public async Task<Pedido> ObterPedidoCliente(int idUsuario, EPedidoStatus pedidoStatus)
        {
            const string sqlPedido =
                @"SELECT Id, IdUsuario, ValorTotal, DataCadastro, PedidoStatus, Validade, DataPedido, DataFechamento, DataUltimaAtualizacao
                    FROM Pedido WHERE IdUsuario = @idUsuario AND PedidoStatus = @pedidoStatus";

            const string sqlPedidoItem =
                @"SELECT Id, IdPedido, IdProduto, ProdutoNome, Quantidade, ValorUnitario, Validade 
                    FROM PedidoItem WHERE IdPedido = @idPedido";

            var pedido = QueryFirstOrDefault<Pedido>(sqlPedido, new { idUsuario, pedidoStatus });
            if (pedido == null)
                return null;

            var idPedido = pedido.Id;
            var pedidosItem = await QueryAsync<PedidoItem>(sqlPedidoItem, new { idPedido });

            foreach (var pedidoItem in pedidosItem) pedido.AdicionarItem(pedidoItem);

            return pedido;
        }

        public async Task<List<Pedido>> ObterPedidoPorIdUsuario(int idUsuario)
        {
            const string sqlPedido =
                @"SELECT Id, IdUsuario, ValorTotal, DataCadastro, PedidoStatus, Validade, DataPedido, DataFechamento, DataUltimaAtualizacao
                    FROM Pedido WHERE IdUsuario = @idUsuario AND PedidoStatus <> 0 and PedidoStatus <> 4 order by Id desc";

            const string sqlPedidoItem =
                @"SELECT Id, IdPedido, IdProduto, ProdutoNome, Quantidade, ValorUnitario, Validade 
                    FROM PedidoItem WHERE IdPedido = @idPedido";

            var pedidos = await QueryAsync<Pedido>(sqlPedido, new { idUsuario });
            pedidos = pedidos.ToList();

            foreach (var pedido in pedidos)
            {
                var idPedido = pedido.Id;
                var pedidosItem = await QueryAsync<PedidoItem>(sqlPedidoItem, new { idPedido });
                foreach (var pedidoItem in pedidosItem) pedido.AdicionarItem(pedidoItem);
            }

            return (List<Pedido>)pedidos;
        }

        public async Task<Pedido> ObterPedidoPorId(int idPedido)
        {
            const string sqlPedido =
                @"SELECT Id, IdUsuario, ValorTotal, DataCadastro, PedidoStatus, Validade, DataPedido, DataFechamento, DataUltimaAtualizacao
                    FROM Pedido WHERE Id = @idPedido";

            const string sqlPedidoItem =
                @"SELECT Id, IdPedido, IdProduto, ProdutoNome, Quantidade, ValorUnitario, Validade 
                    FROM PedidoItem WHERE IdPedido = @idPedido";

            var pedido = QueryFirstOrDefault<Pedido>(sqlPedido, new { idPedido });
            if (pedido == null)
                return null;

            var pedidosItem = await QueryAsync<PedidoItem>(sqlPedidoItem, new { idPedido });

            foreach (var pedidoItem in pedidosItem) pedido.AdicionarItem(pedidoItem);

            return pedido;
        }

        public bool AdicionarPedido(Pedido pedido)
        {
            const string sql =
                @"INSERT INTO Pedido (IdUsuario, ValorTotal, DataCadastro, PedidoStatus, Validade, DataPedido, DataFechamento, DataUltimaAtualizacao) 
                                 VALUE (@IdUsuario, @ValorTotal, @DataCadastro, @PedidoStatus, @Validade, @DataPedido, @DataFechamento, @DataUltimaAtualizacao)";

            var linhasAfetadas = Execute(sql, pedido);
            return linhasAfetadas > 0;
        }

        public bool AtualizarPedido(Pedido pedido)
        {
            const string sql = @"UPDATE Pedido
                                 SET IdUsuario = @IdUsuario, 
                                     ValorTotal = @ValorTotal, 
                                     DataCadastro = @DataCadastro, 
                                     PedidoStatus = @PedidoStatus,
                                     Validade = @Validade,
                                     DataPedido = @DataPedido,
                                     DataFechamento = @DataFechamento,
                                     DataUltimaAtualizacao = @DataUltimaAtualizacao
                                 WHERE Id = @Id";

            var linhasAfetadas = Execute(sql, pedido);
            return linhasAfetadas > 0;
        }

        public bool RemoverPedido(int id)
        {
            const string sql = @"DELETE FROM Pedido WHERE Id = @id";

            var linhasAfetadas = Execute(sql, new { id });
            return linhasAfetadas > 0;
        }

        public async Task<PedidoItem> ObterItemPorId(int id)
        {
            const string sql = @"SELECT Id, IdPedido, IdProduto, ProdutoNome, Quantidade, ValorUnitario
                                 FROM PedidoItem WHERE Id = @id";

            var pedidoItem = await QueryFirstOrDefaultAsync<PedidoItem>(sql, new { id });
            return pedidoItem;
        }

        public async Task<PedidoItem> ObterItemPorPedido(int idPedido, int idProduto)
        {
            const string sql = @"SELECT Id, IdPedido, IdProduto, ProdutoNome, Quantidade, ValorUnitario
                                 FROM PedidoItem WHERE IdPedido = @idPedido AND IdProduto = @idProduto";

            var pedidoItem = await QueryFirstOrDefaultAsync<PedidoItem>(sql, new { idPedido, idProduto });
            return pedidoItem;
        }

        public bool AdicionarItem(PedidoItem pedidoItem)
        {
            const string sql =
                @"INSERT INTO PedidoItem (IdPedido, IdProduto, ProdutoNome, Quantidade, ValorUnitario, Validade)
                                 VALUE (@IdPedido, @IdProduto, @ProdutoNome, @Quantidade, @ValorUnitario, @Validade)";

            var linhasAfetadas = Execute(sql, pedidoItem);
            return linhasAfetadas > 0;
        }

        public bool AtualizarItem(PedidoItem pedidoItem)
        {
            const string sql = @"UPDATE PedidoItem 
                                 SET IdPedido = @IdPedido, 
                                     IdProduto = @IdProduto, 
                                     ProdutoNome = @ProdutoNome, 
                                     Quantidade = @Quantidade, 
                                     ValorUnitario = @ValorUnitario
                                  WHERE Id = @Id";

            var linhasAfetadas = Execute(sql, pedidoItem);
            return linhasAfetadas > 0;
        }

        public bool RemoverItem(PedidoItem pedidoItem)
        {
            const string sql = @"DELETE FROM PedidoItem WHERE Id = @Id";
            var linhasAfetadas = Execute(sql, pedidoItem);
            return linhasAfetadas > 0;
        }

        public async Task<List<int>> ObterPedidosExpirados(string dataAtual)
        {
            const string sql = @"SELECT Id FROM Pedido WHERE PedidoStatus = 1 and Validade < @dataAtual";
            var listaIds = await QueryAsync<int>(sql, new { dataAtual });
            return listaIds.ToList();
        }

        public async Task<List<PedidoViewModel>> ObterTodosOsPedidos()
        {
            const string sql =
                @"SELECT P.Id, U.Nome, U.Email, PS.Nome AS STATUS, P.ValorTotal, P.DataCadastro, P.Validade, P.DataPedido, P.DataFechamento FROM Pedido AS P
                                INNER JOIN Usuario AS U ON U.Id = P.IdUsuario
                                INNER JOIN PedidoStatus AS PS ON PS.Valor = P.PedidoStatus
                                WHERE P.PedidoStatus <> 0
                                ORDER BY P.DataUltimaAtualizacao DESC";

            var Pedidos = await QueryAsync<PedidoViewModel>(sql, new { });

            return Pedidos.ToList();
        }

        public async Task<List<PedidoStatusViewModel>> ObterPedidoStatus()
        {
            const EPedidoStatus rascunho = EPedidoStatus.Rascunho;
            const EPedidoStatus extornado = EPedidoStatus.Extornado;
            const string sql = "SELECT Valor, Nome FROM PedidoStatus WHERE Valor NOT IN(@rascunho, @extornado)";
            var pedidoStatus = await QueryAsync<PedidoStatusViewModel>(sql, new { rascunho, extornado });

            return pedidoStatus.ToList();
        }
    }
}