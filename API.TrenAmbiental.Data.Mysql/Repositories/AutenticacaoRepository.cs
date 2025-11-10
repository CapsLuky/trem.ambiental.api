using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using Microsoft.Extensions.Configuration;

namespace API.TrenAmbiental.Data.Mysql.Repositories
{
    public class AutenticacaoRepository : BaseRepository, IAutenticacaoRepository
    {
        public AutenticacaoRepository(
            IConfiguration config) : base(config)
        {
        }

        public async Task<string> BuscarSenha(string email)
        {
            const string sql = @"SELECT Senha FROM Usuario WHERE Email = @Email";

            return await QueryFirstOrDefaultAsync<string>(sql, new { email });
        }
        
        public async Task<int> ChecarUsuarioAtivo(string email)
        {
            const string sql = @"SELECT Ativo FROM Usuario WHERE Email = @Email";

            var resposta = await QueryFirstOrDefaultAsync<int>(sql, new { email });
            return resposta;
        }

        public async Task<bool> AtualizarSenha(string email, string senhaNova)
        {
            const string sql = @"UPDATE Usuario SET Senha = @senhaNova WHERE Email = @email";

            var linhasAfetadas = await ExecuteAsync(sql, new { email, senhaNova });
            return linhasAfetadas > 0;
        }
    }
}