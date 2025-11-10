using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.TrenAmbiental.Data.Mysql.Interfaces;
using API.TrenAmbiental.DTO.Entidade.Cadastro;
using API.TrenAmbiental.DTO.Enum;
using API.TrenAmbiental.DTO.Model;
using API.TrenAmbiental.DTO.Model.Cadastro;
using API.TrenAmbiental.DTO.ViewModel.Cadastro;
using Microsoft.Extensions.Configuration;

namespace API.TrenAmbiental.Data.Mysql.Repositories
{
    public class CadastroRepository : BaseRepository, ICadastroRepository
    {
        public CadastroRepository(IConfiguration config) : base(config)
        {
        }

        #region Usuario

        public async Task<int> InserirUsuario(UsuarioCadastroModel usuario)
        {
            var sql = @"INSERT INTO Usuario (Nome, Email, IdValorPerfil)
                        VALUE (@nome, @email, @IdValorPerfil);
                        SELECT LAST_INSERT_ID();";

            var ultimoIdInserido = await ExecuteScalarAsync<int>(sql, usuario);


            return ultimoIdInserido;
        }

        public async Task<bool> AtualizarUsuario(CadUsuarioModel usuario)
        {
            var sql = @"UPDATE Usuario 
                         SET Nome = @nome,
                             Apelido = @apelido,
                             Email = @email,
                             Idade = @idade,
                             IdGenero = @idGenero,
                             IdValorPerfil = @IdValorPerfil
                         WHERE Id = @id";

            var linhasAfetadas = await ExecuteAsync(sql, new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Apelido,
                usuario.Email,
                usuario.Idade,
                usuario.IdGenero,
                usuario.IdValorPerfil
            });

            return linhasAfetadas > 0;
        }

        public async Task<bool> AtualizarParcialUsuario(EditarUsuarioModel usuario)
        {
            usuario.IdValorPerfil = usuario.Perfil.Valor;
            
            const string sql = @"UPDATE Usuario 
                         SET Nome = @nome,
                             Email = @email,
                             Ativo = @ativo,
                             IdValorPerfil = @IdValorPerfil
                         WHERE Id = @id";

            var linhasAfetadas = await ExecuteAsync(sql, usuario);

            return linhasAfetadas > 0;
        }

        public async Task<List<Usuario>> BuscarUsuarioList()
        {
            const string sql = @"SELECT Usuario.Id, Nome, Email, IdValorPerfil, NomePerfil, Ativo FROM Usuario
	                    INNER JOIN Perfil AS PE ON PE.Valor = Usuario.IdValorPerfil";

            var resposta = await QueryAsync<Usuario>(sql, new { });
            return resposta.ToList();
        }

        public async Task<UsuarioInfoViewModel> BuscarUsuarioPorId(int id)
        {
            var sql =
                @$"SELECT U.Id, U.Nome, Apelido, Email, Idade, G.Nome AS Genero, IdGenero, P.NomePerfil AS Perfil, IdValorPerfil FROM Usuario AS U
                         LEFT JOIN Genero G ON U.IdGenero = G.Id
                         INNER JOIN Perfil P ON U.IdValorPerfil = P.Id 
                         WHERE U.Id = {id}";

            var resposta = await QueryFirstOrDefaultAsync<UsuarioInfoViewModel>(sql, new { });
            return resposta;
        }

        public async Task<UsuarioInfoViewModel> BuscarUsuarioPorEmail(string email)
        {
            var sql =
                @"SELECT U.Id, U.Nome, Apelido, Email, Idade, G.Nome AS Genero, IdGenero, P.NomePerfil AS Perfil, IdValorPerfil 
                    FROM Usuario AS U
                    LEFT JOIN Genero G ON U.IdGenero = G.Id
                    INNER JOIN Perfil P ON U.IdValorPerfil = P.Id 
                    WHERE U.Email = @email";

            var resposta = await QueryFirstOrDefaultAsync<UsuarioInfoViewModel>(sql, new { email });
            return resposta;
        }

        public async Task<bool> CadastrarPrimeiraSenha(string email, string senha)
        {
            var sql = @"UPDATE Usuario 
                         SET Senha = @senha
                         WHERE Email = @email";

            var linhasAfetadas = await ExecuteAsync(sql, new { email, senha });

            return linhasAfetadas > 0;
        }

        public async Task<AtualizacaoCadastralViewModel> BuscarDadosParaAtualizacaoCadastral(int id)
        {
            var sql =
                @"SELECT U.Nome, Email, Apelido, Idade, G.Nome AS Genero, G.Id AS IdGenero, E.Cep, E.Condominio 
                    FROM Usuario AS U
                    LEFT JOIN Genero G ON U.IdGenero = G.Id
                    LEFT JOIN Endereco E ON E.IdUsuario = U.Id
                    WHERE U.Id = @id";

            var resposta = await QueryFirstOrDefaultAsync<AtualizacaoCadastralViewModel>(sql, new { id });
            return resposta;
        }

        public async Task<bool> AtualizarCadastro(AtualizacaoCadastralViewModel usuario)
        {
            const string sql = @"UPDATE Usuario 
                         SET Nome = @nome,
                             Email = @email,
                             Apelido = @apelido,
                             Idade = @idade,
                             IdGenero = @idGenero
                         WHERE Id = @idUsuario";

            var linhasAfetadas = await ExecuteAsync(sql, usuario);

            return linhasAfetadas > 0;
        }

        public async Task<List<PerfilViewModel>> BuscarPerfilList()
        {
            var sql = @"SELECT Valor, NomePerfil FROM Perfil";

            var resposta = await QueryAsync<PerfilViewModel>(sql, new { });
            return resposta.ToList();
        }
        
        #endregion

        #region Itens reciclaveis

        public async Task<bool> InserirReciclavel(ReciclavelModel reciclavel)
        {
            var sql = @"INSERT INTO Reciclavel (Nome, Ponto, EficienciaEnergetica)
                         VALUE (@nome, @ponto, @EficienciaEnergetica)";

            var linhasAfetadas = await ExecuteAsync(sql, new
            {
                reciclavel.Nome, reciclavel.Ponto, reciclavel.EficienciaEnergetica
            });

            return linhasAfetadas > 0;
        }

        public async Task<bool> AtualizarReciclavel(ReciclavelModel reciclavel)
        {
            var sql = @"UPDATE Reciclavel 
                         SET Nome = @nome,
                             Ponto = @ponto,
                             EficienciaEnergetica = @EficienciaEnergetica,
                             Ativo = @ativo
                         WHERE Id = @id";

            var linhasAfetadas = await ExecuteAsync(sql, reciclavel);

            return linhasAfetadas > 0;
        }

        public async Task<bool> ExcluirReciclavel(int id)
        {
            var sql = @"DELETE FROM Reciclavel WHERE Id = @id";

            var linhasAfetadas = await ExecuteAsync(sql, new { id });

            return linhasAfetadas > 0;
        }

        public async Task<List<ReciclavelViewModel>> BuscarReciclavelList()
        {
            var sql = "SELECT Id, Nome, Ponto, EficienciaEnergetica, Ativo FROM Reciclavel";

            var resposta = await QueryAsync<ReciclavelViewModel>(sql, new { });
            return resposta.ToList();
        }

        public async Task<ReciclavelViewModel> BuscarReciclavel(int id)
        {
            var sql = "SELECT Id, Nome, Ponto, EficienciaEnergetica, Ativo FROM Reciclavel WHERE Id = @id";
            var resposta = await QueryFirstOrDefaultAsync<ReciclavelViewModel>(sql, new { id });
            return resposta;
        }

        #endregion

        #region Endereço

        public async Task<bool> AtualizarEndereco(EnderecoModel endereco)
        {
            var linhasAfetadas = 0;
            const string sqlCheck = @"SELECT COUNT(Id) FROM Endereco WHERE IdUsuario = @idUsuario";
            const string sqlUpdate = @"UPDATE Endereco 
                         SET Cep = @cep,
                             Condominio = @condominio
                         WHERE IdUsuario = @idUsuario";

            const string sqlInsert = @"INSERT INTO Endereco (IdUsuario, Cep, Condominio)
                               VALUE (@idUsuario, @cep, @condominio)";

            var temEndereco = await ExecuteScalarAsync<int>(sqlCheck, endereco);

            if (temEndereco > 0)
                linhasAfetadas = await ExecuteAsync(sqlUpdate, endereco);
            else
                linhasAfetadas = await ExecuteAsync(sqlInsert, endereco);

            return linhasAfetadas > 0;
        }

        public async Task<List<EnderecoViewModel>> BuscarEnderecoList()
        {
            var sql = "SELECT Id, IdUsuario, Logradouro, Cep, Complemento, Numero FROM Endereco";
            var resposta = await QueryAsync<EnderecoViewModel>(sql, new { });
            return resposta.ToList();
        }

        public async Task<EnderecoViewModel> BuscarEnderecoPorCep(string cep)
        {
            var sql = "SELECT Id, IdUsuario, Logradouro, Cep, Complemento, Numero FROM Endereco WHERE Cep = @cep";
            var resposta = await QueryFirstOrDefaultAsync<EnderecoViewModel>(sql, new { cep });
            return resposta;
        }

        #endregion

        #region Parâmetro usuário

        public async Task<bool> PrecisaCompletarCadastro(int idUsuario)
        {
            var parametro = EParametroUsuario.PrimeiroCadastro;
            var sql = @"SELECT Valor 
                        FROM  UsuarioParametro 
                        WHERE IdUsuario = @idUsuario AND Parametro = @parametro";

            var resposta = await QueryFirstOrDefaultAsync<bool>(sql, new { idUsuario, parametro });

            return resposta;
        }

        public async Task<bool> AtualizarParametroUsuario(ParametroUsuarioModel parametro)
        {
            var linhasAfetadas = 0;
            const string sqlCheck = @"SELECT COUNT(Id) FROM UsuarioParametro WHERE IdUsuario = @idUsuario";

            const string sqlUpdate = @"UPDATE UsuarioParametro 
                         SET Valor = @valor
                         WHERE IdUsuario = @idUsuario AND Parametro = @parametro";

            const string sqlInsert = @"INSERT INTO UsuarioParametro (IdUsuario, Parametro, Valor)
                               VALUE (@idUsuario, @parametro, @valor)";

            var temEndereco = await ExecuteScalarAsync<int>(sqlCheck, parametro);

            if (temEndereco > 0)
                linhasAfetadas = await ExecuteAsync(sqlUpdate, parametro);
            else
                linhasAfetadas = await ExecuteAsync(sqlInsert, parametro);

            return linhasAfetadas > 0;
        }

        #endregion
    }
}