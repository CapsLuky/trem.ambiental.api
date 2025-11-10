using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.DTO.Entidade.Catalogo;
using API.TrenAmbiental.DTO.ViewModel.Catalogo;
using Microsoft.Extensions.Configuration;

namespace API.TrenAmbiental.Data.Mysql.Repositories
{
    public class CatalogoRepository: BaseRepository, ICatalogoRepository
    {
        public CatalogoRepository(IConfiguration config) : base(config)
        {

        }

        public async Task<IEnumerable<Produto>> ObterTodosProdutos()
        {
            const string sql =
                @"SELECT Id, Produto.Nome, Descricao, Ativo, ValorPontos, DataCadastro, Imagem, QuantidadeEstoque, QuantidadeEstoqueBaixo, Validade, Tipo, PT.Nome as NomeTipo
                  FROM Produto
                  INNER JOIN ProdutoTipo AS PT ON PT.Valor = Produto.Tipo
                  ORDER BY Id DESC";

            var produto = await QueryAsync<Produto>(sql, new { });
            return produto.ToList();
        }

        public async Task<Produto> ObterProdutoPorId(int id)
        {
            const string sql =
                @"SELECT Id, Nome, Descricao, Ativo, ValorPontos, DataCadastro, Imagem, QuantidadeEstoque, QuantidadeEstoqueBaixo, Validade, Tipo
                                 FROM Produto
                                 WHERE Id = @id";

            var produto = await QueryFirstOrDefaultAsync<Produto>(sql, new { id });
            return produto;
        }

        public bool AdicionarProduto(Produto produto)
        {
            const string sql =
                @"INSERT INTO Produto (Nome, Descricao, Ativo, ValorPontos, DataCadastro, Imagem, QuantidadeEstoque, QuantidadeEstoqueBaixo, Validade, Tipo) 
                                 VALUE (@Nome, @Descricao, @Ativo, @ValorPontos, @DataCadastro, @Imagem, @QuantidadeEstoque, @QuantidadeEstoqueBaixo, @Validade, @Tipo)";

            var linhasAfetadas = Execute(sql, produto);

            return linhasAfetadas > 0;
        }

        public bool AtualizarProduto(Produto produto)
        {
            const string sql = @"UPDATE Produto
                                 SET Nome = @Nome,
                                     Descricao = @Descricao,
                                     Ativo = @Ativo,
                                     ValorPontos = @ValorPontos,
                                     DataCadastro = @DataCadastro,
                                     Imagem = @Imagem,
                                     QuantidadeEstoque = @QuantidadeEstoque,
                                     QuantidadeEstoqueBaixo = @QuantidadeEstoqueBaixo,
                                     Validade = @Validade,
                                     Tipo = @Tipo
                                  WHERE Id = @Id";

            var linhasAfetadas = Execute(sql, produto);
            return linhasAfetadas > 0;
        }

        public bool AtualizarProdutos(List<Produto> produto)
        {
            const string sql = @"UPDATE Produto
                                 SET Nome = @nome,
                                     Descricao = @descricao,
                                     Ativo = @ativo,
                                     ValorPontos = @valorPontos,
                                     DataCadastro = @dataCadastro,
                                     Imagem = @imagem,
                                     QuantidadeEstoque = @quantidadeEstoque,
                                     QuantidadeEstoqueBaixo = @quantidadeEstoqueBaixo,
                                     Validade = @validade,
                                     Tipo = @tipo
                                  WHERE Id = @Id";

            var linhasAfetadas = Execute(sql, produto);
            return linhasAfetadas == produto.Count;
        }

        public async Task<List<ProdutoTipoViewModel>> BuscarProdutoTipoList()
        {
            var sql = @"SELECT Valor, Nome FROM ProdutoTipo";

            var resposta = await QueryAsync<ProdutoTipoViewModel>(sql, new { });
            return resposta.ToList();
        }
    }
}