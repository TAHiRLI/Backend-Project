using System.ComponentModel.DataAnnotations;

namespace Quarter.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        [MaxLength(30)]
        [Required]
        public string Username { get; set; }
        [MaxLength(30)]
        [Required]
        public string Fullname { get; set; }
        [MaxLength(100)]
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [MinLength(8)]
        [MaxLength(20)]
        [Required]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage ="Paswords do not match")]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [MaxLength(20)]
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}
