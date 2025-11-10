using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.DTO.Model.Pesagem;
using API.TrenAmbiental.DTO.ViewModel.Pesagem;
using Microsoft.Extensions.Configuration;

namespace API.TrenAmbiental.Data.Mysql.Repositories
{
    public class PesagemRepository : BaseRepository, IPesagemRepository
    {
        public PesagemRepository(IConfiguration config) : base(config)
        {
        }

        public async Task<List<ItemReciclavelViewModel>> BuscarReciclavelList()
        {
            var sql = "SELECT Id, Nome, Ponto FROM Reciclavel WHERE Ativo = 1";

            var resposta = await QueryAsync<ItemReciclavelViewModel>(sql, new { });
            return resposta.ToList();
        }

        // public async Task<int> BuscarPontosUsuario(int idUsuario)
        // {
        //     const string sql = @"SELECT Pontos FROM UsuarioPonto WHERE idUsuario = @idUsuario";
        //
        //     var resposta = await QueryFirstOrDefaultAsync<int>(sql, new { idUsuario });
        //     return resposta;
        // }
        //
        // public async Task<bool> CreditarUsuario(UsuarioPontoModel credito)
        // {
        //     var linhasAfetadas = 0;
        //     const string sqlCheck = @"SELECT COUNT(Id) FROM UsuarioPonto WHERE idUsuario = @idUsuario";
        //     const string sqlUpdate = @"UPDATE UsuarioPonto SET Pontos = @Pontos WHERE IdUsuario = @IdUsuario";
        //     const string sqlInsert = @"INSERT INTO UsuarioPonto(IdUsuario, Pontos) VALUE(@IdUsuario, @Pontos)";
        //
        //     var temUsuario = await ExecuteScalarAsync<int>(sqlCheck, credito);
        //
        //     if (temUsuario > 0)
        //         linhasAfetadas = await ExecuteAsync(sqlUpdate, credito);
        //     else
        //         linhasAfetadas = await ExecuteAsync(sqlInsert, credito);
        //
        //     return linhasAfetadas > 0;
        // }

        public async Task<List<PesquisaUsuarioViewModel>> PesquisarUsuario(string textoPesquisa)
        {
            int id;
            string sql;

            bool numero = int.TryParse(textoPesquisa, out id);

            if (numero)
            {
                sql = $"SELECT Id, Nome, Email FROM Usuario WHERE Id = {id} AND Ativo = {1}";
            }
            else
            {
                sql = $"SELECT Id, Nome, Email FROM Usuario WHERE Nome LIKE '%{textoPesquisa}%' OR Email LIKE '%{textoPesquisa}%' AND Ativo = {1} LIMIT 5";
            }

            var resposta = await QueryAsync<PesquisaUsuarioViewModel>(sql, new { });
            return resposta.ToList();
        }

        public async Task<bool> GravarHistoricoPesagem(PesagemHistoricoModel pesagemLancamento)
        {
            const string sql = @"INSERT INTO 
                                 PesagemHistorico(IdUsuarioOperador, IdUsuarioCliente, IdReciclavel, Peso, Ponto, DataLancamento, Cancelado, DataCancelado) 
                                 VALUES(@IdUsuarioOperador, @IdUsuarioCliente, @IdReciclavel, @Peso, @Ponto, @DataLancamento, @Cancelado, @DataCancelado)";

            var linhasAfetadas = await ExecuteAsync(sql, pesagemLancamento);

            return linhasAfetadas > 0;
        }

        public async Task<bool> AtualizarHistoricoPesagem(PesagemHistoricoModel pesagemLancamento)
        {
            const string sql = @"UPDATE PesagemHistorico 
                                 SET 
                                    Cancelado = @Cancelado,
                                    DataCancelado = @DataCancelado
                                 WHERE Id = @Id";

            var linhasAfetadas = await ExecuteAsync(sql, pesagemLancamento);

            return linhasAfetadas > 0;
        }

        public async Task<List<PesagemHistoricoViewModel>> BuscarHistoricoPesagemList()
        {
            const string sql =
                @"SELECT P.Id, UO.Nome AS Operador, UC.Nome AS Cliente, R.Nome AS Reciclavel, Peso, P.Ponto, DataLancamento, Cancelado, DataCancelado 
                                FROM PesagemHistorico AS P
                                INNER JOIN Usuario AS UO ON UO.Id = P.IdUsuarioOperador 
                                INNER JOIN Usuario AS UC ON UC.Id = P.IdUsuarioCliente 
                                INNER JOIN Reciclavel R ON R.Id = P.IdReciclavel
                                ORDER BY DataLancamento DESC";

            var pesagemHistoricos = await QueryAsync<PesagemHistoricoViewModel>(sql, new { });

            return pesagemHistoricos.ToList();
        }

        public async Task<List<PesagemHistoricoViewModel>> BuscarHistoricoPesagemList(int idUsuarioCliente)
        {
            const string sql =
                @"SELECT P.Id, UO.Nome AS Operador, UC.Nome AS Cliente, R.Nome AS Reciclavel, Peso, P.Ponto, DataLancamento, Cancelado, DataCancelado 
                                FROM PesagemHistorico AS P
                                INNER JOIN Usuario AS UO ON UO.Id = P.IdUsuarioOperador 
                                INNER JOIN Usuario AS UC ON UC.Id = P.IdUsuarioCliente 
                                INNER JOIN Reciclavel R ON R.Id = P.IdReciclavel
                                WHERE P.IdUsuarioCliente = @idUsuarioCliente
                                AND Cancelado = 0
                                ORDER BY DataLancamento DESC";

            var pesagemHistoricos =
                await QueryAsync<PesagemHistoricoViewModel>(sql, new { idUsuarioCliente });

            return pesagemHistoricos.ToList();
        }

        public async Task<List<PesagemHistoricoViewModel>> BuscarHistoricoPesagemPorDataList(
            PesagemFiltroHistoricoModel filtro)
        {
            var sql =
                @"SELECT P.Id, UO.Nome AS Operador, UC.Nome AS Cliente, R.Nome AS Reciclavel, Peso, P.Ponto, DataLancamento, Cancelado, DataCancelado 
                                FROM PesagemHistorico AS P
                                INNER JOIN Usuario AS UO ON UO.Id = P.IdUsuarioOperador 
                                INNER JOIN Usuario AS UC ON UC.Id = P.IdUsuarioCliente 
                                INNER JOIN Reciclavel R ON R.Id = P.IdReciclavel
                                WHERE (DataLancamento BETWEEN @dataInicio AND @dataFim)";

            if (filtro.idUsuario is not null)
                sql += " AND P.IdUsuarioCliente = @idUsuario AND Cancelado = 0";

            sql += " ORDER BY DataLancamento DESC";

            var pesagemHistoricos =
                await QueryAsync<PesagemHistoricoViewModel>(sql, filtro);

            return pesagemHistoricos.ToList();
        }

        public async Task<int> BuscarPesoUsuario(PesoFiltroModel filtro)
        {
            string sql =
                @"SELECT IFNULL(SUM(Peso),0) FROM PesagemHistorico 
                  WHERE idUsuarioCliente = @idUsuario 
                    AND Cancelado = 0";

            if (filtro.dataInicio is not null && filtro.dataFim is not null)
            {
                sql += " AND (DataLancamento BETWEEN @dataInicio AND @dataFim)";
            }

            return await QueryFirstOrDefaultAsync<int>(sql, filtro);
        }
    }
}