using System.ComponentModel.DataAnnotations;

namespace Quarter.ViewModels
{
    public class ResetPasswordViewModel
    {

        [MinLength(8)]
        [MaxLength(20)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match !!!")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }

    }
}
