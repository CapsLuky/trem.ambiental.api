using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.DTO.Entidade.Pontuacao;
using API.TrenAmbiental.DTO.Model.Pesagem;

namespace API.TrenAmbiental.Domain.Services.Interfaces
{
    public interface IPontuacaoService
    {
        Task<int> BuscarPontuacaoUsuario(PontuacaoFiltroModel filtro);
        Task<int> BuscarSaldoUsuario(int idUsuario);

        //Task<List<Ranking>> BuscarRankingPorMes(DateTime data);
        Task<List<Ranking>> BuscarRanking();
        Task<int> BuscarPosicaoRanking(int idUsuario, bool mesAnterior);
        Task<Ranking> BuscarRankingPorIdUsuarioData(int idUsuario, DateTime data);
        Task<bool> AtualizarRanking(Ranking ranking);
        Task<bool> AdicionarRanking(Ranking ranking);
    }
}