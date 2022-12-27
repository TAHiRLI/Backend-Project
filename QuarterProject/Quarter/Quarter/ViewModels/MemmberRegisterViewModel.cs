using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Quarter.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Quarter.ViewModels
{
    public class MemmberRegisterViewModel
    {
        [MaxLength(30)]
        public string Fullname { get; set; }
        public string Username { get; set; }
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [MinLength(8)]
        [MaxLength(20)]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [MinLength(8)]
        [MaxLength(20)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match !!!")]
        public string ConfirmPassword { get; set; }
        public bool IsSubscribed { get; set; }
        [AllowedFileTypes("image/jpeg", "image/png")]
        [MaxFileSize(2)]
        public IFormFile? File { get; set; }
    }
}
