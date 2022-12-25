using System.ComponentModel.DataAnnotations;

namespace Quarter.Areas.Admin.ViewModels
{
    public class AdminLoginViewModel
    {
        
        public string Username { get; set; }
        [MaxLength(20)]
        public string Password { get; set; }    
    }
}
