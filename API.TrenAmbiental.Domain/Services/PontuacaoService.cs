using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Entidade.Pontuacao;
using API.TrenAmbiental.DTO.Enum;
using API.TrenAmbiental.DTO.Model.Pesagem;
using Microsoft.Extensions.Logging;

namespace API.TrenAmbiental.Domain.Services
{
    public class PontuacaoService : IPontuacaoService
    {
        private readonly INotificador _notificador;
        private readonly IPontuacaoRepository _pontuacaoRepository;
        private readonly ILogger<PontuacaoService> _logger;

        public PontuacaoService(
            INotificador notificador,
            ILogger<PontuacaoService> logger,
            IPontuacaoRepository pontuacaoRepository)
        {
            _notificador = notificador;
            _pontuacaoRepository = pontuacaoRepository;
            _logger = logger;
        }

        public async Task<int> BuscarPontuacaoUsuario(PontuacaoFiltroModel filtro)
        {
            try
            {
                var filtrado = new PontuacaoFiltroModel();
                
                if (filtro.dataInicio is not null && filtro.dataFim is not null)
                {
                    filtrado.idUsuario = filtro.idUsuario;
                    filtrado.dataInicio = new DateTime(filtro.dataInicio.Value.Year, 
                                                       filtro.dataInicio.Value.Month,
                                                       filtro.dataInicio.Value.Day);
                    
                    filtrado.dataFim = new DateTime(filtro.dataFim.Value.Year, 
                                                    filtro.dataFim.Value.Month,
                                                    filtro.dataFim.Value.Day,23,59,59);
                }
                else
                {
                    filtrado.idUsuario = filtro.idUsuario;
                }

                return await _pontuacaoRepository.BuscarPontuacaoUsuario(filtrado);
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao buscar pontuação",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao buscar pontuação, idUsuario: {filtro.idUsuario}. {e.Message}", e);
                return -1;
            }
        }

        public async Task<int> BuscarSaldoUsuario(int idUsuario)
        {
            try
            {
                return await _pontuacaoRepository.BuscarSaldoUsuario(idUsuario);
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao buscar pontuação",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao buscar pontuação, idUsuario: {idUsuario}. {e.Message}", e);
                return 0;
            }
        }

        // public async Task<List<Ranking>> BuscarRankingPorMes(DateTime data)
        // {
        //     try
        //     {
        //         return await _pontuacaoRepository.BuscarRankingPorMes(data.Month, data.Year);
        //     }
        //     catch (Exception e)
        //     {
        //         _notificador.Handle(new Notificacao("Erro", "Erro ao buscar ranking",
        //             ETipoNotificacao.error.ToString()));
        //         _logger.LogError($"Erro ao buscar ranking, datetime: {data}. {e.Message}", e);
        //         return null;
        //     }
        // }

        public async Task<List<Ranking>> BuscarRanking()
        {
            try
            {
                return await _pontuacaoRepository.BuscarRanking();
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao buscar ranking geral",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao buscar ranking geral. {e.Message}", e);
                return null;
            }
        }

        public async Task<int> BuscarPosicaoRanking(int idUsuario, bool mesAnterior)
        {
            try
            {
                var dataAtual = DataHoraAtual.ObterDataHoraServidorWindows();

                if (mesAnterior) dataAtual = dataAtual.AddMonths(-1);

                var posicoes = await _pontuacaoRepository.BuscarPosicaoRanking(dataAtual.Month, dataAtual.Year);

                var posicao = posicoes.Find(posicao => posicao.IdUsuario == idUsuario);

                return posicao?.Posicao ?? 0;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Não foi possível buscar posição no ranking.",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError(
                    $"Erro ao buscar ranking, IdUsuario: {idUsuario}. {e.Message}",
                    e);
                return 0;
            }
        }

        public async Task<Ranking> BuscarRankingPorIdUsuarioData(int idUsuario, DateTime data)
        {
            try
            {
                return await _pontuacaoRepository.BuscarRankingPorIdUsuarioData(idUsuario, data.Month, data.Year);
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Não foi possível buscar ranking do usuário.",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError(
                    $"Erro ao buscar ranking, IdUsuario: {idUsuario}, Data: {data}. {e.Message}",
                    e);
                return null;
            }
        }

        public async Task<bool> AtualizarRanking(Ranking ranking)
        {
            try
            {
                return await _pontuacaoRepository.AtualizarRanking(ranking);
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Tivemos um problema ao atualizar ranking",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao atualizar ranking, idUsuario: {ranking.IdUsuario}, " +
                                 $"datahora: {ranking.DataAtualizacao}, " +
                                 $"pontos: {ranking.Pontos}. {e.Message}", e);
                return false;
            }
        }

        public async Task<bool> AdicionarRanking(Ranking ranking)
        {
            try
            {
                return await _pontuacaoRepository.AdicionarRanking(ranking);
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Tivemos um problema ao adicionar ranking",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao adicionar ranking, idUsuario: {ranking.IdUsuario}, " +
                                 $"datahora: {ranking.DataAtualizacao}, " +
                                 $"pontos: {ranking.Pontos}. {e.Message}", e);
                return false;
            }
        }
    }
}