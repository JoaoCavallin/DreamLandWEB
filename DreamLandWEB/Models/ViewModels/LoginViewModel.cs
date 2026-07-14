using System.ComponentModel.DataAnnotations;

namespace DreamLandWEB.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.Password)]
        [StringLength(100)]
        public string Senha { get; set; }
    }
}
