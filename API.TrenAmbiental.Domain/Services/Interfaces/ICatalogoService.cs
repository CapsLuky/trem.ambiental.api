using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.DTO.Entidade.Carrinho;
using API.TrenAmbiental.DTO.Model.Catalogo;
using API.TrenAmbiental.DTO.ViewModel.Catalogo;

namespace API.TrenAmbiental.Domain.Services.Interfaces
{
    public interface ICatalogoService
    {
        Task<bool> UploadFoto(UploadImagemModel uploadImagemModel);
        bool AdicionarProduto(ProdutoModel produto);
        Task<bool> AtualizarProduto(ProdutoModel produto);
        Task<ProdutoViewModel> ObterProdutoPorId(int produtoId);
        Task<List<ProdutoViewModel>> ObterTodos(string canal);

        Task<bool> DebitarEstoque(int produtoId, int quantidade);
        Task<bool> ReporEstoque(int produtoId, int quantidade);
        Task<bool> DebitarListaProdutosPedido(ListaProdutosPedido produtoLista);
        Task<bool> ReporListaProdutosPedido(ListaProdutosPedido lista);
        Task<List<ProdutoTipoViewModel>> BuscarProdutoTipoList();
    }
}