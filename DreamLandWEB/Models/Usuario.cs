using System.ComponentModel.DataAnnotations;

namespace DreamLandWEB.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Formato de e-mail inválido")]
        [StringLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public string SenhaHash { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Informe um telefone válido, apenas números (DDD + número)")]
        [StringLength(11)]
        public string? Telefone { get; set; }
        public bool Admin { get; set; } = false;

        public ICollection<Pedido>? Pedidos { get; set; }
    }
}
