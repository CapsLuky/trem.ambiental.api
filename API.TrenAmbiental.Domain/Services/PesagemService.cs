using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Entidade.Pontuacao;
using API.TrenAmbiental.DTO.Enum;
using API.TrenAmbiental.DTO.Model.Pesagem;
using API.TrenAmbiental.DTO.ViewModel.Pesagem;
using Microsoft.Extensions.Logging;

namespace API.TrenAmbiental.Domain.Services
{
    public class PesagemService : IPesagemService
    {
        private readonly INotificador _notificador;
        private readonly IPesagemRepository _pesagemRepository;
        private readonly IPontuacaoService _pontuacaoService;
        private readonly ILogger<PesagemService> _logger;

        public PesagemService(
            INotificador notificador,
            IPontuacaoService pontuacaoService,
            IPesagemRepository pesagemRepository,
            ILogger<PesagemService> logger)
        {
            _notificador = notificador;
            _pontuacaoService = pontuacaoService;
            _pesagemRepository = pesagemRepository;
            _logger = logger;
        }

        public async Task<List<ItemReciclavelViewModel>> BuscarReciclavelList()
        {
            try
            {
                return await _pesagemRepository.BuscarReciclavelList();
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao buscar lista de recilaveis. {e.Message}", e);
                return null;
            }
        }

        public async Task<List<PesquisaUsuarioViewModel>> PesquisarUsuario(string textoPesquisa)
        {
            try
            {
                return await _pesagemRepository.PesquisarUsuario(textoPesquisa);
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao pesquisar usuario. Texto {textoPesquisa}. {e.Message}", e);
                return null;
            }
        }

        public async Task<bool> GravarHistoricoPesagem(PesagemHistoricoModel pesagemLancamento)
        {
            try
            {
                pesagemLancamento.SetarDataDeLancamento();

                var gravado = await _pesagemRepository.GravarHistoricoPesagem(pesagemLancamento);

                if (gravado)
                {
                    var ranking = await _pontuacaoService.BuscarRankingPorIdUsuarioData(
                        pesagemLancamento.IdUsuarioCliente, DataHoraAtual.ObterDataHoraServidorWindows());

                    if (ranking == null)
                    {
                        ranking = Ranking.RankingFactory.NovoRanking(pesagemLancamento.IdUsuarioCliente,
                            pesagemLancamento.Ponto);
                        await _pontuacaoService.AdicionarRanking(ranking);
                    }
                    else
                    {
                        ranking.AdicionarPontos(pesagemLancamento.Ponto);
                        await _pontuacaoService.AtualizarRanking(ranking);
                    }

                    return gravado;
                }

                _notificador.Handle(new Notificacao("Atenção",
                    "Talvez a pesagem não foi gravada, verifique o histórico.",
                    ETipoNotificacao.warning.ToString()));

                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $@"Erro ao gravar histórico de lançamento IdUsuarioLogado: {pesagemLancamento.IdUsuarioOperador}, 
                                    IdUsuarioCliente: {pesagemLancamento.IdUsuarioCliente}. {e.Message}", e);

                _notificador.Handle(new Notificacao("Erro", "Erro ao gravar histórico de lançamento",
                    ETipoNotificacao.error.ToString()));
                return false;
            }
        }

        public async Task<bool> HistoricoPesagemCancelarLancameno(int idLancamento)
        {
            try
            {
                var pesagemLancamento = new PesagemHistoricoModel
                {
                    Id = idLancamento,
                    Cancelado = true
                };
                pesagemLancamento.SetarDataDeCancelamento();

                var retorno = await _pesagemRepository.AtualizarHistoricoPesagem(pesagemLancamento);

                _notificador.Handle(retorno
                    ? new Notificacao("Sucesso", "Lançamento cancelado.", ETipoNotificacao.success.ToString())
                    : new Notificacao("Atenção", "Lançamento não foi cancelado",
                        ETipoNotificacao.warning.ToString()));
                
                return retorno;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $@"Erro ao atualizar histórico de lançamento idLancamento: {idLancamento}. {e.Message}", e);
                _notificador.Handle(new Notificacao("Erro", "Erro ao atualizar histórico de lançamento",
                    ETipoNotificacao.error.ToString()));
                return false;
            }
        }

        public async Task<List<PesagemHistoricoViewModel>> BuscarHistoricoPesagemList()
        {
            try
            {
                return await _pesagemRepository.BuscarHistoricoPesagemList();
            }
            catch (Exception e)
            {
                _logger.LogError($@"Erro ao buscar histórico de lançamento. {e.Message}", e);
                _notificador.Handle(new Notificacao("Erro", "Erro ao buscar histórico de lançamento",
                    ETipoNotificacao.error.ToString()));
                return new List<PesagemHistoricoViewModel>();
            }
        }

        public async Task<List<PesagemHistoricoViewModel>> BuscarHistoricoPesagemList(int idUsuarioCliente)
        {
            try
            {
                return await _pesagemRepository.BuscarHistoricoPesagemList(idUsuarioCliente);
            }
            catch (Exception e)
            {
                _logger.LogError($@"Erro ao buscar histórico de lançamento por cliente. {e.Message}", e);
                _notificador.Handle(new Notificacao("Erro", "Erro ao buscar histórico de lançamento",
                    ETipoNotificacao.error.ToString()));
                return new List<PesagemHistoricoViewModel>();
            }
        }

        public async Task<List<PesagemHistoricoViewModel>> BuscarHistoricoPesagemPorDataList(
            PesagemFiltroHistoricoModel filtro)
        {
            try
            {
                var historico = await _pesagemRepository.BuscarHistoricoPesagemPorDataList(filtro);

                // var notificacao2 = new Notificacao("Pronto!", $"Encontramos {historico.Count} itens", ETipoNotificacao.warning.ToString());
                // _notificador.Handle(notificacao2, HttpStatusCode.OK);

                if (historico.Count == 0)
                {
                    var notificacao1 =
                        new Notificacao("Ops!", "Nenhum item encontrado", ETipoNotificacao.info.ToString());
                    _notificador.Handle(notificacao1);
                }
                else
                {
                    var notificacao2 = new Notificacao("Pronto!", $"Encontramos {historico.Count} itens",
                        ETipoNotificacao.info.ToString());
                    _notificador.Handle(notificacao2, HttpStatusCode.NotFound);
                }

                return historico;
            }
            catch (Exception e)
            {
                _logger.LogError($@"Erro ao buscar histórico de lançamento. {e.Message}", e);

                _notificador.Handle(
                    new Notificacao("Erro", "Erro ao buscar histórico de lançamento",
                        ETipoNotificacao.error.ToString()), HttpStatusCode.BadRequest);
                
                return new List<PesagemHistoricoViewModel>();
            }
        }

        public bool ValidarAlgumaCoisa()
        {
            //-------------------
            // FAZ ALGUMA VALIDAÇÃO...
            //-------------------

            var Invalido = false;

            if (Invalido)
            {
                var notificacao1 = new Notificacao("Ops!", "Informações inválida", ETipoNotificacao.error.ToString());
                _notificador.Handle(notificacao1, HttpStatusCode.NotFound);

                return false;
            }

            var notificacao2 = new Notificacao("Pronto!", "Validação deu certo", ETipoNotificacao.success.ToString());
            _notificador.Handle(notificacao2);

            return true;
        }

        public async Task<int> BuscarPesoUsuario(PesoFiltroModel filtro)
        {
            try
            {
                var filtrado = new PesoFiltroModel();

                if (filtro.dataInicio is not null && filtro.dataFim is not null)
                {
                    filtrado.idUsuario = filtro.idUsuario;
                    filtrado.dataInicio = new DateTime(filtro.dataInicio.Value.Year,
                        filtro.dataInicio.Value.Month,
                        filtro.dataInicio.Value.Day);

                    filtrado.dataFim = new DateTime(filtro.dataFim.Value.Year,
                        filtro.dataFim.Value.Month,
                        filtro.dataFim.Value.Day, 23, 59, 59);
                }
                else
                {
                    filtrado.idUsuario = filtro.idUsuario;
                }

                return await _pesagemRepository.BuscarPesoUsuario(filtrado);
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao buscar peso", ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao buscar peso, idUsuario: {filtro.idUsuario}. {e.Message}", e);
                return -1;
            }
        }
    }
}