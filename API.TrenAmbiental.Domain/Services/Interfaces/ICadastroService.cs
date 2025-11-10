using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.DTO.Model;
using API.TrenAmbiental.DTO.Model.Cadastro;
using API.TrenAmbiental.DTO.ViewModel.Cadastro;

namespace API.TrenAmbiental.Domain.Services.Interfaces
{
    public interface ICadastroService
    {
        #region Usuario

        Task<bool> InserirUsuario(UsuarioCadastroModel usuario);
        Task<bool> AtualizarUsuario(CadUsuarioModel usuario);
        Task<bool> AtualizarParcialUsuario(EditarUsuarioModel usuario);
        Task<List<UsuarioViewModel>> BuscarUsuarioList();
        Task<UsuarioInfoViewModel> BuscarUsuario(int id);
        Task<AtualizacaoCadastralViewModel> BuscarDadosParaAtualizacaoCadastral(int id);
        Task<bool> AtualizarCadastro(AtualizacaoCadastralViewModel usuario);
        Task<List<PerfilViewModel>> BuscarPerfilList();
        #endregion

        #region Reciclavel

        Task<bool> InserirReciclavel(ReciclavelModel reciclavel);
        Task<bool> AtualizarReciclavel(ReciclavelModel reciclavel);
        Task<bool> ExcluirReciclavel(int id);
        Task<List<ReciclavelViewModel>> BuscarReciclavelList();
        Task<ReciclavelViewModel> BuscarReciclavel(int id);

        #endregion

        #region Endereço

        //Task<bool> InserirEndereco(EnderecoInbound endereco);
        Task<bool> AtualizarEndereco(EnderecoModel endereco);
        Task<List<EnderecoViewModel>> BuscarEnderecoList();
        Task<EnderecoViewModel> BuscarEnderecoPorCep(string cep);

        #endregion

        #region Parâmetro usuário

        Task<bool> PrecisaCompletarCadastro(int idUsuario);
        Task<bool> AtualizarParametroUsuario(ParametroUsuarioModel parametro);

        #endregion
    }
}