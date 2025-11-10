using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.DTO.Entidade.Pontuacao;
using API.TrenAmbiental.DTO.Model.Pesagem;
using Microsoft.Extensions.Configuration;

namespace API.TrenAmbiental.Data.Mysql.Repositories
{
    public class PontuacaoRepository : BaseRepository, IPontuacaoRepository
    {
        public PontuacaoRepository(IConfiguration config) : base(config)
        {
        }

        public async Task<int> BuscarPontuacaoUsuario(PontuacaoFiltroModel filtro)
        {
            string sql =
                @"SELECT IFNULL(SUM(Ponto),0) FROM PesagemHistorico 
                  WHERE idUsuarioCliente = @idUsuario 
                    AND Cancelado = 0";

            if (filtro.dataInicio is not null && filtro.dataFim is not null)
            {
                sql += " AND (DataLancamento BETWEEN @dataInicio AND @dataFim)";
            }

            return await QueryFirstOrDefaultAsync<int>(sql, filtro);
        }

        public async Task<int> BuscarSaldoUsuario(int idUsuario)
        {
            const string sql = @"SELECT 
                    (SELECT IFNULL(SUM(Ponto),0) FROM PesagemHistorico WHERE idUsuarioCliente = @idUsuario AND Cancelado = 0) 
                    - 
                    (SELECT IFNULL(SUM(ValorTotal),0) FROM Pedido where idUsuario = @idUsuario AND PedidoStatus IN (1,2,3)) AS Saldo";

            return await QueryFirstOrDefaultAsync<int>(sql, new { idUsuario });
        }

        // public async Task<List<Ranking>> BuscarRankingPorMes(int mes, int ano)
        // {
        //     const string sql = @"SELECT Ranking.Id, IdUsuario, U.Nome AS NomeUsuario, U.Email, Pontos From Ranking
        //                          INNER JOIN Usuario as U ON U.Id = Ranking.IdUsuario
        //                          WHERE  MONTH(Ranking.DataAtualizacao) = @mes AND YEAR(Ranking.DataAtualizacao) = @ano
        //                          ORDER BY Pontos DESC";
        //
        //     var ranking = await QueryAsync<Ranking>(sql, new { mes, ano });
        //     return ranking.ToList();
        // }

        public async Task<List<PosicaoRanking>> BuscarPosicaoRanking(int mes, int ano)
        {
            const string sql = @"SELECT 
	                            (@row_number := @row_number + 1) AS `Posicao`, t.IdUsuario
                                FROM `Ranking` AS t, (SELECT @row_number := 0) r
                                WHERE 
	                                 (MONTH(t.DataAtualizacao) = @mes AND YEAR(t.DataAtualizacao) = @ano)
                                ORDER BY Pontos DESC ";

            var posicao = await QueryAsync<PosicaoRanking>(sql, new { mes, ano });
            return posicao.ToList();
        }

        public async Task<List<Ranking>> BuscarRanking()
        {
            const string sql =
                @"SELECT U.Id AS IdUsuario, U.Nome AS NomeUsuario, U.Email, Pontos, DataAtualizacao From Ranking
                                 INNER JOIN Usuario as U ON U.Id = Ranking.IdUsuario
                                 WHERE U.IdValorPerfil <> 4
                                 ORDER BY MONTH(DataAtualizacao) DESC, Pontos DESC";

            var ranking = await QueryAsync<Ranking>(sql, new { });
            return ranking.ToList();
        }

        public async Task<Ranking> BuscarRankingPorIdUsuarioData(int idUsuario, int mes, int ano)
        {
            const string sql =
                @"SELECT Ranking.Id, U.Nome AS NomeUsuario, U.Email, IdUsuario, Pontos, DataAtualizacao FROM Ranking
                                 INNER JOIN Usuario as U ON U.Id = Ranking.IdUsuario                                 
                                 WHERE IdUsuario = @idUsuario AND MONTH(DataAtualizacao) = @mes AND YEAR(DataAtualizacao) = @ano";
        
            return await QueryFirstOrDefaultAsync<Ranking>(sql, new { idUsuario, mes, ano });
        }

        public async Task<bool> AtualizarRanking(Ranking ranking)
        {
            const string sql = @"UPDATE Ranking
                                 SET IdUsuario = @IdUsuario, 
                                     Pontos = @Pontos, 
                                     DataAtualizacao = @DataAtualizacao
                                 WHERE Id = @Id";

            var linhasAfetadas = await ExecuteAsync(sql, ranking);
            return linhasAfetadas > 0;
        }

        public async Task<bool> AdicionarRanking(Ranking ranking)
        {
            const string sql = @"INSERT INTO Ranking (IdUsuario, Pontos, DataAtualizacao)
                                    VALUE (@IdUsuario, @Pontos, @DataAtualizacao)";

            var linhasAfetadas = await ExecuteAsync(sql, ranking);
            return linhasAfetadas > 0;
        }
    }
}