using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.DTO.Model.Pesagem;
using API.TrenAmbiental.DTO.ViewModel.Pesagem;

namespace API.TrenAmbiental.Domain.Services.Interfaces
{
    public interface IPesagemService
    {
        Task<List<PesquisaUsuarioViewModel>> PesquisarUsuario(string textoPesquisa);
        Task<List<ItemReciclavelViewModel>> BuscarReciclavelList();

        //Task<bool> CreditarUsuario(UsuarioPontoModel credito);
        Task<bool> GravarHistoricoPesagem(PesagemHistoricoModel pesagemLancamento);
        Task<bool> HistoricoPesagemCancelarLancameno(int idLancamento);
        Task<List<PesagemHistoricoViewModel>> BuscarHistoricoPesagemList();
        Task<List<PesagemHistoricoViewModel>> BuscarHistoricoPesagemList(int idUsuarioCliente);
        Task<List<PesagemHistoricoViewModel>> BuscarHistoricoPesagemPorDataList(PesagemFiltroHistoricoModel filtro);
        Task<int> BuscarPesoUsuario(PesoFiltroModel filtro);
        bool ValidarAlgumaCoisa();
    }
}