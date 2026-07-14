using System.ComponentModel.DataAnnotations;

namespace DreamLandWEB.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        [StringLength(100)]
        public string Senha { get; set; }
    }
}
