using System.Threading.Tasks;

namespace API.TrenAmbiental.Data.Mysql.Interfaces
{
    public interface IAutenticacaoRepository
    {
        Task<string> BuscarSenha(string email);
        Task<bool> AtualizarSenha(string email, string senhaNova);
        Task<int> ChecarUsuarioAtivo(string email);
    }
}