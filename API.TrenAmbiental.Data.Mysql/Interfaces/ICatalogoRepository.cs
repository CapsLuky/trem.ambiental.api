using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.DTO.Entidade.Catalogo;
using API.TrenAmbiental.DTO.ViewModel.Catalogo;

namespace API.TrenAmbiental.Data.Mysql.Interfaces
{
    public interface ICatalogoRepository
    {
        Task<IEnumerable<Produto>> ObterTodosProdutos();
        Task<Produto> ObterProdutoPorId(int id);
        bool AdicionarProduto(Produto produto);
        bool AtualizarProduto(Produto produto);
        bool AtualizarProdutos(List<Produto> produto);
        Task<List<ProdutoTipoViewModel>> BuscarProdutoTipoList();
    }
}