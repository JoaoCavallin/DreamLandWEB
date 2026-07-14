using DreamLandWEB.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace DreamLandWEB.Models
{
    public class Produto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }
        public string Descricao { get; set; }
        [Required]
        public CategoriaProduto Categoria { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }
        public string Tamanho { get; set; } // RN, P, M, G, 2, 4, 6 anos...
        [Required]
        public CondicaoProduto Condicao { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "O estoque deve ser maior que zero.")]
        public int Estoque { get; set; }
        public bool Disponivel { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public string ImagemUrl { get; set; }
    }
}
