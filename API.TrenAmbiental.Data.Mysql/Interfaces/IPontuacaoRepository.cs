using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.DTO.Entidade.Pontuacao;
using API.TrenAmbiental.DTO.Model.Pesagem;

namespace API.TrenAmbiental.Data.Mysql.Interfaces
{
    public interface IPontuacaoRepository
    {
        Task<int> BuscarPontuacaoUsuario(PontuacaoFiltroModel filtro);
        Task<int> BuscarSaldoUsuario(int idUsuario);

        //Task<List<Ranking>> BuscarRankingPorMes(int mes, int ano);
        Task<List<PosicaoRanking>> BuscarPosicaoRanking(int mes, int ano);
        Task<List<Ranking>> BuscarRanking();
        Task<Ranking> BuscarRankingPorIdUsuarioData(int idUsuario, int mes, int ano);
        Task<bool> AtualizarRanking(Ranking ranking);
        Task<bool> AdicionarRanking(Ranking ranking);
    }
}