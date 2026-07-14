using System.ComponentModel.DataAnnotations;

namespace DreamLandWEB.Models.ViewModels
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; }

        [Required, EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Informe um telefone válido, apenas números (DDD + número)")]
        [StringLength(11)]
        public string? Telefone { get; set; }

        [Required, DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        public string Senha { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarSenha { get; set; }
    }
}
