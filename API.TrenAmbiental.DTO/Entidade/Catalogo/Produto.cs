using System;
using API.TrenAmbiental.DTO.DomainObjects;
using API.TrenAmbiental.DTO.Model.Catalogo;
using Dapper.Contrib.Extensions;

namespace API.TrenAmbiental.DTO.Entidade.Catalogo
{
    [Table("Produto")]
    public class Produto
    {
        [Key]
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public bool Ativo { get; private set; }
        public int ValorPontos { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public string Imagem { get; private set; }
        public int QuantidadeEstoque { get; private set; }
        public short QuantidadeEstoqueBaixo { get; private set; }
        public short Validade { get; private set; }
        public short Tipo { get; private set; }
        public string NomeTipo { get; private set; }

        protected Produto()
        {
        }

        public Produto(int id, string nome, string descricao, bool ativo, int valorPontos, DateTime dataCadastro,
            string imagem, short quantidadeEstoque, short quantidadeEstoqueBaixo, short validade)
        {
            Nome = nome;
            Descricao = descricao;
            Ativo = ativo;
            ValorPontos = valorPontos;
            DataCadastro = dataCadastro;
            Imagem = imagem;
            QuantidadeEstoque = quantidadeEstoque;
            QuantidadeEstoqueBaixo = quantidadeEstoqueBaixo;
            Validade = validade;
            Id = id;

            Validar();
        }

        public Produto(string nome, string descricao, bool ativo, int valorPontos, DateTime dataCadastro,
            short quantidadeEstoque, short quantidadeEstoqueBaixo, short validade)
        {
            Nome = nome;
            Descricao = descricao;
            Ativo = ativo;
            ValorPontos = valorPontos;
            DataCadastro = dataCadastro;
            QuantidadeEstoque = quantidadeEstoque;
            QuantidadeEstoqueBaixo = quantidadeEstoqueBaixo;
            Validade = validade;

            Validar();
        }

        public void Ativar() => Ativo = true;

        public void Desativar() => Ativo = false;

        public void DebitarEstoque(int quantidade)
        {
            if (quantidade < 0) quantidade *= -1;
            if (!PossuiEstoque(quantidade)) throw new DomainException("Estoque insuficiente");
            QuantidadeEstoque -= quantidade;
        }

        public void ReporEstoque(int quantidade)
        {
            QuantidadeEstoque += quantidade;
        }

        public bool PossuiEstoque(int quantidade)
        {
            return QuantidadeEstoque >= quantidade;
        }

        public void AtualizarProduto(ProdutoModel produto)
        {
            Id = produto.Id;
            Nome = produto.Nome;
            Descricao = produto.Descricao == "" ? null : produto.Descricao;
            Ativo = produto.Ativo;
            ValorPontos = produto.ValorPontos;
            QuantidadeEstoque = produto.QuantidadeEstoque;
            QuantidadeEstoqueBaixo = produto.QuantidadeEstoqueBaixo;
            Validade = produto.Validade;
            Tipo = produto.Tipo;
        }

        private static string AdicionarDescricao(string descricao)
        {
            return descricao == string.Empty ? null : descricao;
        }

        public static implicit operator Produto(ProdutoModel produtoModel)
        {
            return new Produto
            {
                Nome = produtoModel.Nome,
                Descricao = AdicionarDescricao(produtoModel.Descricao),
                ValorPontos = produtoModel.ValorPontos,
                QuantidadeEstoque = produtoModel.QuantidadeEstoque,
                QuantidadeEstoqueBaixo = produtoModel.QuantidadeEstoqueBaixo,
                Tipo = produtoModel.Tipo,
                Validade = produtoModel.Validade,
                DataCadastro = DataHoraAtual.ObterDataHoraServidorWindows()
            };
        }

        public void Validar()
        {
            Validacoes.ValidarSeVazio(Nome, "O campo Nome do produto não pode estar vazio");
            Validacoes.ValidarSeVazio(Descricao, "O campo Descricao do produto não pode estar vazio");
            Validacoes.ValidarSeMenorQue(ValorPontos, 1, "O campo Valor do produto não pode se menor igual a 0");
            Validacoes.ValidarSeMenorQue(QuantidadeEstoque, 1, "O campo Valor do estoque não pode se menor igual a 0");
        }
    }
}