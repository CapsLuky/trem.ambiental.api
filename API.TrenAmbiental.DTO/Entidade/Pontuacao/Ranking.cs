using System;
using API.TrenAmbiental.DTO.DomainObjects;

namespace API.TrenAmbiental.DTO.Entidade.Pontuacao;

public class Ranking
{
    public int Id { get; private set; }
    public int IdUsuario { get; private set; }
    public string NomeUsuario { get; private set; }
    public string Email { get; private set; }
    public int Pontos { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    public void DebitarPontos(int quantidade)
    {
        Pontos -= quantidade;
        DataAtualizacao = DataHoraAtual.ObterDataHoraServidorWindows();
    }

    public void AdicionarPontos(int quantidade)
    {
        Pontos += quantidade;
        DataAtualizacao = DataHoraAtual.ObterDataHoraServidorWindows();
    }

    public static class RankingFactory
    {
        public static Ranking NovoRanking(int idUsuario, int pontos)
        {
            var ranking = new Ranking
            {
                IdUsuario = idUsuario,
                DataAtualizacao = DataHoraAtual.ObterDataHoraServidorWindows(),
                Pontos = pontos
            };

            return ranking;
        }
    }
}