using System;
using System.ComponentModel.DataAnnotations;
using API.TrenAmbiental.DTO.Entidade.Catalogo;

namespace API.TrenAmbiental.DTO.ViewModel.Catalogo
{
    public class ProdutoViewModel
    {
        public int Id { get; private set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Nome { get; private set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Descricao { get; private set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public bool Ativo { get; private set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int ValorPontos { get; private set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime DataCadastro { get; private set; }
        public string Imagem { get; private set; }

        [Range(1, int.MaxValue, ErrorMessage = "O campo {0} precisa ter o valor mínimo de {1}")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int QuantidadeEstoque { get; private set; }
        public short QuantidadeEstoqueBaixo { get; private set; }
        public short Validade { get; private set; }
        public ProdutoTipo Tipo { get; private set; }

        public static implicit operator ProdutoViewModel(Produto produto)
        {
            return new ProdutoViewModel
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Ativo = produto.Ativo,
                ValorPontos = produto.ValorPontos,
                DataCadastro = produto.DataCadastro,
                Imagem = produto.Imagem,
                QuantidadeEstoque = produto.QuantidadeEstoque,
                QuantidadeEstoqueBaixo = produto.QuantidadeEstoqueBaixo,
                Validade = produto.Validade,
                Tipo = new ProdutoTipo
                {
                    Nome = produto.NomeTipo,
                    Valor = (short)produto.Tipo
                }
            };
        }
    }
}