using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Entidade.Carrinho;
using API.TrenAmbiental.DTO.Entidade.Venda;
using API.TrenAmbiental.DTO.Enum;
using API.TrenAmbiental.DTO.Enum.Venda;
using API.TrenAmbiental.DTO.Model.Carrinho;
using API.TrenAmbiental.DTO.Model.Catalogo;
using API.TrenAmbiental.DTO.ViewModel.Carrinho;
using Microsoft.Extensions.Logging;

namespace API.TrenAmbiental.Domain.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly INotificador _notificador;
        private readonly ILogger<PedidoService> _logger;
        private readonly IPedidoRepository _pedidoRepository;
        private static ICatalogoService _catalogoService;
        private readonly ICatalogoRepository _catalogoRepository;
        private readonly IPontuacaoRepository _pontuacaoRepository;
        private readonly ICadastroRepository _cadastroRepository;

        public PedidoService(INotificador notificador,
            ICatalogoService catalogoService,
            IPedidoRepository pedidoRepository,
            ICatalogoRepository catalogoRepository,
            IPontuacaoRepository pontuacaoRepository,
            ICadastroRepository cadastroRepository,
            ILogger<PedidoService> logger)
        {
            _notificador = notificador;
            _logger = logger;
            _catalogoService = catalogoService;
            _pedidoRepository = pedidoRepository;
            _catalogoRepository = catalogoRepository;
            _pontuacaoRepository = pontuacaoRepository;
            _cadastroRepository = cadastroRepository;
        }

        public async Task<bool> IniciarPedido(int idUsuario)
        {
            try
            {
                var pedido = await _pedidoRepository.ObterPedidoCliente(idUsuario, EPedidoStatus.Rascunho);
                var itensList = new List<Item>();
                pedido.PedidoItems.ForEach(i =>
                    itensList.Add(new Item { Id = i.IdProduto, Quantidade = i.Quantidade }));
                var listaProdutosPedido = new ListaProdutosPedido { IdPedido = pedido.Id, Itens = itensList };
                var estoqueDebitado = await _catalogoService.DebitarListaProdutosPedido(listaProdutosPedido);

                if (estoqueDebitado)
                {
                    var temSaldoSuficiente = await ValidaSaldoDePontos(pedido);

                    if (temSaldoSuficiente)
                    {
                        pedido.IniciarPedido();
                        _pedidoRepository.AtualizarPedido(pedido);
                        return true;
                    }

                    pedido.TornarRascunho();
                    _pedidoRepository.AtualizarPedido(pedido);
                    await _catalogoService.ReporListaProdutosPedido(listaProdutosPedido);
                    return false;
                }

                pedido.TornarRascunho();
                _pedidoRepository.AtualizarPedido(pedido);
                
                return false;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Não foi possível efetura o pedido",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao iniciar pedido, idUsuario: {idUsuario}. {e.Message}", e);
                return false;
            }
        }

        public async Task<bool> FinalizarPedido(FinalizarPedidoModel finalizarPedidoModel)
        {
            try
            {
                var pedido = await _pedidoRepository.ObterPedidoPorId(finalizarPedidoModel.IdPedido);

                if (pedido == null)
                {
                    _notificador.Handle(new Notificacao("", "Pedido não encontrado", ETipoNotificacao.info.ToString()));
                    return false;
                }

                if (pedido.PedidoStatus == EPedidoStatus.Rascunho)
                {
                    _notificador.Handle(new Notificacao("Atenção", "Este pedido ainda não foi solicitado pelo cliente",
                        ETipoNotificacao.warning.ToString()));
                    return false;
                }

                if (pedido.PedidoStatus != EPedidoStatus.EmAberto)
                {
                    _notificador.Handle(new Notificacao("", "Este pedido já foi finalizado",
                        ETipoNotificacao.warning.ToString()));
                    return false;
                }

                var itensList = new List<Item>();
                pedido.PedidoItems.ForEach(i =>
                    itensList.Add(new Item { Id = i.IdProduto, Quantidade = i.Quantidade }));
                var listaProdutosPedido = new ListaProdutosPedido { IdPedido = pedido.Id, Itens = itensList };

                pedido.MudarStatusDoPedido(finalizarPedidoModel.PedidoStatus);

                var pedidoAtualizado = _pedidoRepository.AtualizarPedido(pedido);
                if (!pedidoAtualizado)
                {
                    _notificador.Handle(new Notificacao("Erro", "Problema ao atualizar status do pedido",
                        ETipoNotificacao.error.ToString()));
                    return false;
                }

                if (finalizarPedidoModel.PedidoStatus == EPedidoStatus.Entregue)
                {
                    _notificador.Handle(new Notificacao("Tudo certo", "Pedido finalizado com sucesso",
                        ETipoNotificacao.success.ToString()));
                    return true;
                }

                var listaReposta = await _catalogoService.ReporListaProdutosPedido(listaProdutosPedido);
                if (listaReposta)
                {
                    _notificador.Handle(new Notificacao("Pedido finalizado", "Produtos repostos no estoque.",
                        ETipoNotificacao.success.ToString()));
                    return true;
                }

                pedido.IniciarPedido();
                _pedidoRepository.AtualizarPedido(pedido);
                _notificador.Handle(new Notificacao("Erro",
                    "Problema ao repor produtos no estoque, o pedido não foi finalizado",
                    ETipoNotificacao.error.ToString()));
                return false;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Não foi possível finalizar o pedido",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError(
                    $"Erro ao finalizar pedido, idPedido: {finalizarPedidoModel.IdPedido}, pedidoStatus: {finalizarPedidoModel.PedidoStatus}. {e.Message}",
                    e);
                return false;
            }
        }

        private async Task<bool> ValidaSaldoDePontos(Pedido pedido)
        {
            var saldoDePontos = await _pontuacaoRepository.BuscarSaldoUsuario(pedido.IdUsuario);
            if (saldoDePontos >= pedido.ValorTotal)
            {
                return true;
            }

            _notificador.Handle(new Notificacao("", "Saldo insuficiente para realizar o pedido",
                ETipoNotificacao.warning.ToString()));
            return false;
        }

        public async Task<bool> EnviarItemPedidoParaCarrinho(PedidoItemModel pedidoItemModel)
        {
            try
            {
                var pedido =
                    await _pedidoRepository.ObterPedidoCliente(pedidoItemModel.IdUsuario, EPedidoStatus.Rascunho);

                if (pedido == null)
                    return await AdicionarItemPedido(pedidoItemModel);

                var pedidoItemExisteNoCarrinho = pedido.PedidoItemExistente(pedidoItemModel.IdProduto);

                if (!pedidoItemExisteNoCarrinho) return await AdicionarItemPedido(pedidoItemModel);

                _notificador.Handle(new Notificacao("", "Você já adicionou este item ao carrinho",
                    ETipoNotificacao.warning.ToString()));
                return false;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("", "Erro ao adicioanar item", ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao enviar item para carrinho, idUsuario: {pedidoItemModel.IdUsuario}, " +
                                 $"idProduto: {pedidoItemModel.IdProduto}, quantidade: {pedidoItemModel.Quantidade}  {e.Message}",
                    e);
                return false;
            }
        }

        public async Task<bool> AdicionarItemPedido(PedidoItemModel pedidoItemModel)
        {
            try
            {
                var produtoViewModel = await _catalogoService.ObterProdutoPorId(pedidoItemModel.IdProduto);
                var pedido =
                    await _pedidoRepository.ObterPedidoCliente(pedidoItemModel.IdUsuario, EPedidoStatus.Rascunho);
                PedidoItem pedidoItem = produtoViewModel;
                pedidoItem.AdicionarUnidades(pedidoItemModel.Quantidade);

                if (pedido != null && pedido.PedidoItems.Any(item =>
                        item.IdProduto == produtoViewModel.Id && produtoViewModel.QuantidadeEstoque <= item.Quantidade))
                {
                    _notificador.Handle(new Notificacao("", $"{produtoViewModel.Nome} com estoque insuficiente",
                        ETipoNotificacao.warning.ToString()));
                    return false;
                }

                if (pedido == null)
                {
                    pedido = Pedido.PedidoFactory.NovoPedidoRascunho(pedidoItemModel.IdUsuario);
                    _pedidoRepository.AdicionarPedido(pedido);

                    pedido = await _pedidoRepository.ObterPedidoCliente(pedidoItemModel
                        .IdUsuario, EPedidoStatus.Rascunho);
                }

                var pedidoItemExistente = pedido.PedidoItemExistente(pedidoItem.IdProduto);

                pedido.AdicionarItem(pedidoItem);
                pedido.CalcularValidadePedido();

                if (pedidoItemExistente)
                    _pedidoRepository.AtualizarItem(
                        pedido.PedidoItems.FirstOrDefault(p => p.IdProduto == pedidoItem.IdProduto));
                else
                    _pedidoRepository.AdicionarItem(pedidoItem);

                _pedidoRepository.AtualizarPedido(pedido);

                return true;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("", "Erro ao adicioanar item", ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao adicioanar item, idUsuario: {pedidoItemModel.IdUsuario}, " +
                                 $"idProduto: {pedidoItemModel.IdProduto}, quantidade: {pedidoItemModel.Quantidade}  {e.Message}",
                    e);
                return false;
            }
        }

        public async Task<bool> AtualizarItemPedido(AtualizarItemPedidoModel atualizarItemPedidoModel)
        {
            try
            {
                var pedido =
                    await _pedidoRepository.ObterPedidoCliente(atualizarItemPedidoModel
                        .IdUsuario, EPedidoStatus.Rascunho);

                if (pedido == null)
                {
                    _notificador.Handle(new Notificacao("", "Pedido não encontrado!",
                        ETipoNotificacao.info.ToString()));
                    return false;
                }

                var pedidoItem =
                    await _pedidoRepository.ObterItemPorPedido(pedido.Id,
                        atualizarItemPedidoModel.IdProduto);

                if (!pedido.PedidoItemExistente(pedidoItem.IdProduto))
                {
                    _notificador.Handle(new Notificacao("", "Item do pedido não encontrado!",
                        ETipoNotificacao.info.ToString()));
                    return false;
                }

                pedido.AtualizarUnidades(pedidoItem, atualizarItemPedidoModel.Quantidade);

                var produto = await _catalogoService.ObterProdutoPorId(atualizarItemPedidoModel.IdProduto);
                if (pedido.PedidoItems.Any(item =>
                        item.IdProduto == produto.Id && produto.QuantidadeEstoque < item.Quantidade))
                {
                    _notificador.Handle(new Notificacao("", $"{produto.Nome} com estoque insuficiente",
                        ETipoNotificacao.warning.ToString()));
                    return false;
                }

                _pedidoRepository.AtualizarItem(pedidoItem);
                _pedidoRepository.AtualizarPedido(pedido);

                return true;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Problema ao atualizar item",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError(
                    $"Erro ao atualizar item, idUsuario: {atualizarItemPedidoModel.IdUsuario}, idProduto: {atualizarItemPedidoModel.IdProduto} {e.Message}",
                    e);
                return false;
            }
        }

        public async Task<bool> RemoverItemPedido(RemoverItemPedidoModel removerItemPedidoModel)
        {
            try
            {
                var produto = await _catalogoRepository.ObterProdutoPorId(removerItemPedidoModel.IdProduto);

                if (produto == null)
                {
                    _notificador.Handle(new Notificacao("", "Produto não encontrado",
                        ETipoNotificacao.info.ToString()));
                    return false;
                }

                var pedido =
                    await _pedidoRepository.ObterPedidoCliente(removerItemPedidoModel
                        .IdUsuario, EPedidoStatus.Rascunho);

                if (pedido == null)
                {
                    _notificador.Handle(new Notificacao("", "Pedido não encontrado!",
                        ETipoNotificacao.info.ToString()));
                    return false;
                }

                var pedidoItem =
                    await _pedidoRepository.ObterItemPorPedido(pedido.Id, removerItemPedidoModel.IdProduto);

                if (pedidoItem != null && !pedido.PedidoItemExistente(pedidoItem.IdProduto))
                {
                    _notificador.Handle(new Notificacao("", "Item do pedido não encontrado!",
                        ETipoNotificacao.info.ToString()));
                    return false;
                }

                if (pedidoItem == null)
                {
                    _notificador.Handle(new Notificacao("", "Item do pedido não encontrado!",
                        ETipoNotificacao.info.ToString()));
                    return false;
                }

                pedido.RemoverItem(pedidoItem);
                pedido.CalcularValidadePedido();

                _pedidoRepository.RemoverItem(pedidoItem);
                _pedidoRepository.AtualizarPedido(pedido);

                if (pedido.PedidoItems.Count == 0) _pedidoRepository.RemoverPedido(pedido.Id);

                return true;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Problema ao remover item pedido",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError(
                    $"Erro ao remover item pedido, idProduto: {removerItemPedidoModel.IdProduto}, idUsuario {removerItemPedidoModel.IdUsuario} {e.Message}",
                    e);
                return false;
            }
        }

        public async Task<CarrinhoViewModel> ObterCarrinhoCliente(int idUsuario)
        {
            try
            {
                var pedido = await _pedidoRepository.ObterPedidoCliente(idUsuario, EPedidoStatus.Rascunho);
                if (pedido == null) return null;

                var carrinho = new CarrinhoViewModel
                {
                    IdUsuario = pedido.IdUsuario,
                    ValorTotal = pedido.ValorTotal,
                    IdPedido = pedido.Id,
                    Validade = pedido.Validade
                };

                foreach (var item in pedido.PedidoItems)
                    carrinho.Items.Add(new CarrinhoItemViewModel
                    {
                        IdProduto = item.IdProduto,
                        ProdutoNome = item.ProdutoNome,
                        Quantidade = item.Quantidade,
                        ValorUnitario = item.ValorUnitario,
                        ValorTotal = item.ValorUnitario * item.Quantidade
                    });

                return carrinho;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Problema ao obter carrinho",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao obter carrinho, idUsuario {idUsuario} {e.Message}", e);
                return null;
            }
        }

        public async Task<IEnumerable<SituacaoPedidoViewModel>> ObterPedidoPorIdUsuario(int idUsuario)
        {
            try
            {
                var pedidos = await _pedidoRepository.ObterPedidoPorIdUsuario(idUsuario);

                if (!pedidos.Any()) return null;

                var carrinhoLista = new List<SituacaoPedidoViewModel>();

                foreach (var pedido in pedidos)
                {
                    var carrinho = new SituacaoPedidoViewModel
                    {
                        IdUsuario = pedido.IdUsuario,
                        ValorTotal = pedido.ValorTotal,
                        PedidoStatus = pedido.PedidoStatus,
                        IdPedido = pedido.Id,
                        Validade = pedido.Validade,
                        DataPedido = pedido.DataPedido
                    };

                    foreach (var item in pedido.PedidoItems)
                        carrinho.Items.Add(new CarrinhoItemViewModel
                        {
                            IdProduto = item.IdProduto,
                            ProdutoNome = item.ProdutoNome,
                            Quantidade = item.Quantidade,
                            ValorUnitario = item.ValorUnitario,
                            ValorTotal = item.ValorUnitario * item.Quantidade
                        });

                    carrinhoLista.Add(carrinho);
                }

                return carrinhoLista.OrderByDescending(pedido => pedido.DataPedido);
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Problema ao obter histórico de pedidos",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao obter histórico de pedidos, idUsuario {idUsuario} {e.Message}", e);
                return null;
            }
        }

        public async Task<SituacaoPedidoViewModel> ObterSituacaoPedido(int idPedido)
        {
            try
            {
                var pedido = await _pedidoRepository.ObterPedidoPorId(idPedido);

                if (pedido is null || pedido.PedidoStatus == EPedidoStatus.Rascunho)
                {
                    _notificador.Handle(new Notificacao("Pesquisa concluida", "Nenhum pedido encontrado",
                        ETipoNotificacao.info.ToString()));
                    return null;
                }
                
                var usuario = await _cadastroRepository.BuscarUsuarioPorId(pedido.IdUsuario);
                
                var situacaoPedido = new SituacaoPedidoViewModel
                {
                    IdUsuario = pedido.IdUsuario,
                    NomeUsuario = usuario.Nome,
                    ValorTotal = pedido.ValorTotal,
                    PedidoStatus = pedido.PedidoStatus,
                    IdPedido = pedido.Id,
                    Validade = pedido.Validade,
                    DataPedido = pedido.DataPedido
                };

                foreach (var item in pedido.PedidoItems)
                    situacaoPedido.Items.Add(new CarrinhoItemViewModel
                    {
                        IdProduto = item.IdProduto,
                        ProdutoNome = item.ProdutoNome,
                        Quantidade = item.Quantidade,
                        ValorUnitario = item.ValorUnitario,
                        ValorTotal = item.ValorUnitario * item.Quantidade
                    });

                return situacaoPedido;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Problema ao obter ao obter pedido",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao obter pedido, idPedido: {idPedido} {e.Message}", e);
                return null;
            }
        }

        public async Task<int> ExpirarPedidosForaDaValidade()
        {
            var idPedidoAtual = "";
            var pedidosProcessados = 0;
            var listaIdsPedidos = new List<int>();
            try
            {
                var dataAtual = DateOnly.FromDateTime(DataHoraAtual.ObterDataHoraServidorWindows());
                var dataAtualFormatada = $"{dataAtual.Year}-{dataAtual.Month}-{dataAtual.Day} 00:00:00";
                listaIdsPedidos = await _pedidoRepository.ObterPedidosExpirados(dataAtualFormatada);

                foreach (var idPedido in listaIdsPedidos)
                {
                    idPedidoAtual = idPedido.ToString();
                    await Task.Delay(120);

                    var finalizarPedido = new FinalizarPedidoModel
                    {
                        IdPedido = idPedido,
                        PedidoStatus = EPedidoStatus.Expirado
                    };

                    var processadoComSucesso = await FinalizarPedido(finalizarPedido);
                    if (processadoComSucesso) pedidosProcessados += 1;
                }

                _notificador.ApagarNotificacoes();
                _notificador.Handle(new Notificacao("Finalizado",
                    $"Encontrados: {listaIdsPedidos.Count()}. Processados {pedidosProcessados}",
                    ETipoNotificacao.info.ToString()));
                return pedidosProcessados;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro",
                    $"Problema de processamento. Pedidos expirados encontrados: {listaIdsPedidos.Count()}, pedidos processados {pedidosProcessados}",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError(
                    $"Erro ao expirar pedidos, ultimo pedido processado, idPedido: {idPedidoAtual}, qtd processados: {pedidosProcessados} {e.Message}",
                    e);
                return pedidosProcessados;
            }
        }

        public async Task<List<PedidoViewModel>> ObterTodosOsPedidos()
        {
            try
            {
                return await _pedidoRepository.ObterTodosOsPedidos();
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Problema ao obter os pedidos",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao obter os pedidos. {e.Message}", e);

                return null;
            }
        }

        public async Task<List<PedidoStatusViewModel>> ObterPedidoStatus()
        {
            try
            {
                return await _pedidoRepository.ObterPedidoStatus();
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Problema ao obter status do pedido",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao obter status do pedido. {e.Message}", e);

                return null;
            }
        }
    }
}