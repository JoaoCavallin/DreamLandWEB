using DreamLandWEB.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamLandWEB.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; }

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required]
        public CategoriaProduto Categoria { get; set; }

        [Range(0, 99999.99, ErrorMessage = "O preço deve estar entre R$ 0 e R$ 99.999,99")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        [StringLength(20)]
        public string Tamanho { get; set; }

        [Required]
        public CondicaoProduto Condicao { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo")]
        public int Estoque { get; set; }

        public bool Disponivel { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Required]
        [StringLength(300)]
        [Url(ErrorMessage = "Informe uma URL válida")]
        public string ImagemUrl { get; set; }

        public int? FornecedorId { get; set; }
        public Usuario? Fornecedor { get; set; }

        [StringLength(100)]
        public string? Marca { get; set; }
    }
}