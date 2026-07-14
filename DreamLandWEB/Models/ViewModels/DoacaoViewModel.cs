using DreamLandWEB.Enums;
using System.ComponentModel.DataAnnotations;

namespace DreamLandWEB.Models.ViewModels
{
    public class DoacaoViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; }

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required]
        public CategoriaProduto Categoria { get; set; }

        [Range(0, 99999, ErrorMessage = "O preço deve ser positivo")]
        public decimal Preco { get; set; }

        [Required]
        [StringLength(10)]
        public string Tamanho { get; set; }

        [Required]
        public CondicaoProduto Condicao { get; set; }

        [Required]
        [StringLength(300)]
        [Url(ErrorMessage = "Informe uma URL válida")]
        public string? ImagemUrl { get; set; }
    }
}