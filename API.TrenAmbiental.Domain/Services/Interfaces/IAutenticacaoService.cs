using System.Threading.Tasks;
using API.TrenAmbiental.DTO.Model.Autenticacao;
using API.TrenAmbiental.DTO.ViewModel.Cadastro;

namespace API.TrenAmbiental.Domain.Services.Interfaces
{
    public interface IAutenticacaoService
    {
        Task<bool> ChecarUsuarioAtivo(string email);
        Task<UsuarioInfoViewModel> BuscarInfoUsuario(string email);
        Task<bool> ChecarUsuarioSenha(string email, string senha);
        string CriptografarSenha(string senha);
        Task<bool> AlterarSenha(AlterarSenhaModel alterarSenha);
        UsuarioInfoViewModel GerarJwt(UsuarioInfoViewModel usuarioInfo);
        void EnviarEmailComTokenParaResetarSenha(string email);
        string ValidarToken(string token);
        void EnviarEmailComNovaSenha(string email, string novaSenha);
        string GerarSenhaAleatoria();
    }
}