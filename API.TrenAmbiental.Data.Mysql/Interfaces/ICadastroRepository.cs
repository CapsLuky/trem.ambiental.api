using System.Collections.Generic;
using System.Threading.Tasks;
using API.TrenAmbiental.DTO.Entidade.Cadastro;
using API.TrenAmbiental.DTO.Model;
using API.TrenAmbiental.DTO.Model.Cadastro;
using API.TrenAmbiental.DTO.ViewModel.Cadastro;

namespace API.TrenAmbiental.Data.Mysql.Interfaces
{
    public interface ICadastroRepository
    {
        #region Usuario

        Task<int> InserirUsuario(UsuarioCadastroModel usuario);
        Task<bool> AtualizarUsuario(CadUsuarioModel usuario);
        Task<bool> AtualizarParcialUsuario(EditarUsuarioModel usuario);
        Task<List<Usuario>> BuscarUsuarioList();
        Task<UsuarioInfoViewModel> BuscarUsuarioPorId(int id);
        Task<UsuarioInfoViewModel> BuscarUsuarioPorEmail(string email);
        Task<bool> CadastrarPrimeiraSenha(string email, string senha);
        Task<AtualizacaoCadastralViewModel> BuscarDadosParaAtualizacaoCadastral(int id);
        Task<bool> AtualizarCadastro(AtualizacaoCadastralViewModel usuario);
        Task<List<PerfilViewModel>> BuscarPerfilList();

        #endregion

        #region Reciclaveis

        Task<bool> InserirReciclavel(ReciclavelModel reciclavel);
        Task<bool> AtualizarReciclavel(ReciclavelModel reciclavel);
        Task<bool> ExcluirReciclavel(int id);
        Task<List<ReciclavelViewModel>> BuscarReciclavelList();
        Task<ReciclavelViewModel> BuscarReciclavel(int id);

        #endregion

        #region Endereço

        Task<bool> AtualizarEndereco(EnderecoModel endereco);
        Task<List<EnderecoViewModel>> BuscarEnderecoList();
        Task<EnderecoViewModel> BuscarEnderecoPorCep(string cep);

        #endregion

        #region Usuário parâmetro

        Task<bool> PrecisaCompletarCadastro(int idUsuario);
        Task<bool> AtualizarParametroUsuario(ParametroUsuarioModel parametro);

        #endregion
    }
}