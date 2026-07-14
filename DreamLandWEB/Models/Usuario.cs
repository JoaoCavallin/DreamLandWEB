using System.ComponentModel.DataAnnotations;

namespace DreamLandWEB.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        public string SenhaHash { get; set; }

        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Informe um telefone válido, apenas números (DDD + número)")]
        [StringLength(11)]
        public string? Telefone { get; set; }
        public bool Admin { get; set; } = false;

        public ICollection<Pedido>? Pedidos { get; set; }
    }
}
