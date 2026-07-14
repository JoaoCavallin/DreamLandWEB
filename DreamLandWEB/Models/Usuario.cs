using System.ComponentModel.DataAnnotations;

namespace DreamLandWEB.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string SenhaHash { get; set; }

        public string? Telefone { get; set; }
        public bool Admin { get; set; } = false;

        public ICollection<Pedido>? Pedidos { get; set; }
    }
}
