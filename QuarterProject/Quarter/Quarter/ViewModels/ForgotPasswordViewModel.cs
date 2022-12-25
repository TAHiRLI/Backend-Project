using System.ComponentModel.DataAnnotations;

namespace Quarter.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [MaxLength(100)]
        public string Email { get; set; }
    }
}
