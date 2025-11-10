using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.Domain.Services.Interfaces;
using API.TrenAmbiental.DTO;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Entidade.Carrinho;
using API.TrenAmbiental.DTO.Entidade.Catalogo;
using API.TrenAmbiental.DTO.Enum;
using API.TrenAmbiental.DTO.Enum.Catalogo;
using API.TrenAmbiental.DTO.Model.Catalogo;
using API.TrenAmbiental.DTO.ViewModel.Catalogo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.TrenAmbiental.Domain.Services
{
    public class CatalogoService : ICatalogoService
    {
        private static AppSettings _appSettings;
        private readonly INotificador _notificador;
        private readonly ILogger<CatalogoService> _logger;
        private readonly ICatalogoRepository _catalogoRepository;
        private readonly IEmailService _emailService;

        public CatalogoService(
            IOptions<AppSettings> appSettings,
            INotificador notificador,
            ICatalogoRepository catalogoRepository,
            IEmailService emailService,
            ILogger<CatalogoService> logger)
        {
            _appSettings = appSettings.Value;
            _notificador = notificador;
            _catalogoRepository = catalogoRepository;
            _emailService = emailService;
            _logger = logger;
        }

        #region Gerenciar Produtos

        public bool AdicionarProduto(ProdutoModel produtoModel)
        {
            try
            {
                Produto produto = produtoModel;

                var inserido = _catalogoRepository.AdicionarProduto(produto);

                _notificador.Handle(inserido
                    ? new Notificacao("Sucesso", $"{produto.Nome} cadastrado.", ETipoNotificacao.success.ToString())
                    : new Notificacao("Atenção", $"{produto.Nome} não foi cadastrado",
                        ETipoNotificacao.warning.ToString()));

                return inserido;
            }
            catch (Exception e)
            {
                _notificador.Handle(e.TargetSite?.DeclaringType?.Name == "Validacoes"
                    ? new Notificacao("", e.Message, ETipoNotificacao.warning.ToString())
                    : new Notificacao("Erro", "Erro Inserir produto", ETipoNotificacao.error.ToString()));

                _logger.LogError($"Erro Inserir produto: {produtoModel.Nome}, {e.Message}", e);
                return false;
            }
        }

        public async Task<bool> AtualizarProduto(ProdutoModel produtoModel)
        {
            try
            {
                var produto = await _catalogoRepository.ObterProdutoPorId(produtoModel.Id);

                if (produto == null) return false;

                produto.AtualizarProduto(produtoModel);

                var atualizado = _catalogoRepository.AtualizarProduto(produto);

                if (atualizado)
                    _notificador.Handle(new Notificacao("Sucesso", $"Produto {produto.Nome} Atualizado",
                        ETipoNotificacao.success.ToString()));
                else
                    _notificador.Handle(new Notificacao("Atenção", $"Produto {produto.Nome} não foi atualizado",
                        ETipoNotificacao.warning.ToString()));

                return atualizado;
            }
            catch (Exception e)
            {
                _notificador.Handle(e.TargetSite?.DeclaringType?.Name == "Validacoes"
                    ? new Notificacao("", e.Message, ETipoNotificacao.warning.ToString())
                    : new Notificacao("Erro", "Erro ao atualizar produto", ETipoNotificacao.error.ToString()));

                _logger.LogError($"Erro ao atualizar produto, {e.Message}", e);
                return false;
            }
        }

        public async Task<ProdutoViewModel> ObterProdutoPorId(int produtoId)
        {
            try
            {
                return await _catalogoRepository.ObterProdutoPorId(produtoId);
            }
            catch (Exception e)
            {
                _notificador.Handle(e.TargetSite?.DeclaringType?.Name == "Validacoes"
                    ? new Notificacao("", e.Message, ETipoNotificacao.warning.ToString())
                    : new Notificacao("Erro", "Erro ao buscar produto", ETipoNotificacao.error.ToString()));

                _logger.LogError($"Erro ao buscar produto, {e.Message}", e);
                return null;
            }
        }

        public async Task<List<ProdutoViewModel>> ObterTodos(string canal)
        {
            try
            {
                var produtos = await _catalogoRepository.ObterTodosProdutos();

                if (canal == "app")
                    return produtos.Select(p => (ProdutoViewModel)p).Where(p => p.Ativo)
                        .OrderByDescending(p => p.QuantidadeEstoque).ToList();

                return produtos.Select(produto => (ProdutoViewModel)produto).ToList();
            }
            catch (Exception e)
            {
                _notificador.Handle(e.TargetSite?.DeclaringType?.Name == "Validacoes"
                    ? new Notificacao("", e.Message, ETipoNotificacao.warning.ToString())
                    : new Notificacao("Erro", "Erro ao buscar lista de produtos", ETipoNotificacao.error.ToString()));

                _logger.LogError($"Erro ao buscar lista de produtos, {e.Message}", e);
                return null;
            }
        }

        public async Task<List<ProdutoTipoViewModel>> BuscarProdutoTipoList()
        {
            try
            {
                return await _catalogoRepository.BuscarProdutoTipoList();
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro ao buscar lista do tipo do produto",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro ao buscar lista do tipo do produto. {e.Message}", e);
                throw;
            }
        }

        public async Task<bool> UploadFoto(UploadImagemModel uploadImagemModel)
        {
            try
            {
                var caminhoFotos = _appSettings.CaminhoFotos;
                var filePath = Path.Combine(caminhoFotos, uploadImagemModel.IdProduto + ".jpg");
                await File.WriteAllBytesAsync(filePath, uploadImagemModel.File);
                return true;
            }
            catch (Exception e)
            {
                _notificador.Handle(new Notificacao("Erro", "Erro no updload do arquivo",
                    ETipoNotificacao.error.ToString()));
                _logger.LogError($"Erro upload arquivo. {e.Message}", e);
                return false;
            }
        }

        #endregion

        #region Debitar Estoque

        public async Task<bool> DebitarEstoque(int produtoId, int quantidade)
        {
            var produto = await DebitarItemEstoque(produtoId, quantidade);
            if (produto == null) return false;
            var atualizado = _catalogoRepository.AtualizarProduto(produto);
            return atualizado;
        }

        private async Task<Produto> DebitarItemEstoque(int produtoId, int quantidade)
        {
            try
            {
                var produto = await _catalogoRepository.ObterProdutoPorId(produtoId);

                if (produto == null) return null;

                if (!produto.PossuiEstoque(quantidade))
                {
                    _notificador.Handle(new Notificacao("", $"{produto.Nome} com estoque insuficiente",
                        ETipoNotificacao.warning.ToString()));
                    return null;
                }

                produto.DebitarEstoque(quantidade);

                if (produto.QuantidadeEstoque <= produto.QuantidadeEstoqueBaixo)
                {
                    var emailEstoqueBaixo = _appSettings.EmailEstoqueBaixo;
                    var assunto = $"Estoque baixo, {produto.Nome}";
                    var body = $"O produto {produto.Nome} está com {produto.QuantidadeEstoque} itens no estoque.";

                    _emailService.EnviarEmail(emailEstoqueBaixo, assunto, body, "");
                }

                if (produto.Tipo == (short)EProdutoTipo.Temporario && produto.QuantidadeEstoque < 1)
                    produto.Desativar();

                return produto;
            }
            catch (Exception e)
            {
                _notificador.Handle(e.TargetSite?.DeclaringType?.Name == "Validacoes"
                    ? new Notificacao("", e.Message, ETipoNotificacao.warning.ToString())
                    : new Notificacao("Erro", "Erro ao debitar estoque", ETipoNotificacao.error.ToString()));

                _logger.LogError($"Erro ao debitar estoque, produtoId {produtoId} qtd {quantidade}, {e.Message}", e);
                return null;
            }
        }

        public async Task<bool> DebitarListaProdutosPedido(ListaProdutosPedido produtoLista)
        {
            var produtos = new List<Produto>();
            foreach (var item in produtoLista.Itens)
            {
                var produto = await DebitarItemEstoque(item.Id, item.Quantidade);
                if (produto == null) return false;
                produtos.Add(produto);
            }

            var atualizado = _catalogoRepository.AtualizarProdutos(produtos);
            return atualizado;
        }

        #endregion

        #region Repor Estoque

        public async Task<bool> ReporListaProdutosPedido(ListaProdutosPedido lista)
        {
            var produtos = new List<Produto>();
            foreach (var item in lista.Itens)
            {
                var produto = await ReporItemEstoque(item.Id, item.Quantidade);
                if (produto == null) return false;
                produtos.Add(produto);
            }

            var atualizado = _catalogoRepository.AtualizarProdutos(produtos);
            return atualizado;
        }

        public async Task<bool> ReporEstoque(int produtoId, int quantidade)
        {
            var produto = await ReporItemEstoque(produtoId, quantidade);

            if (produto == null) return false;

            var atualizado = _catalogoRepository.AtualizarProduto(produto);
            return atualizado;
        }

        private async Task<Produto> ReporItemEstoque(int produtoId, int quantidade)
        {
            try
            {
                var produto = await _catalogoRepository.ObterProdutoPorId(produtoId);

                if (produto == null) return null;
               
                produto.ReporEstoque(quantidade);

                if (produto.Tipo == (short)EProdutoTipo.Temporario && produto.QuantidadeEstoque > 0) produto.Ativar();

                return produto;
            }
            catch (Exception e)
            {
                _notificador.Handle(e.TargetSite?.DeclaringType?.Name == "Validacoes"
                    ? new Notificacao("", e.Message, ETipoNotificacao.warning.ToString())
                    : new Notificacao("Erro", "Erro ao repor estoque", ETipoNotificacao.error.ToString()));

                _logger.LogError($"Erro ao repor estoque, produtoId {produtoId}, qtd {quantidade}, {e.Message}", e);
                return null;
            }
        }

        #endregion
    }
}