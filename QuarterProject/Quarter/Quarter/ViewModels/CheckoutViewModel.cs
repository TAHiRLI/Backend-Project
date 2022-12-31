using Quarter.Models;
using System.ComponentModel.DataAnnotations;

namespace Quarter.ViewModels
{
    public class CheckoutViewModel
    {
        public House? House { get; set; }
        public AppUser? AppUser { get; set; }
        [MaxLength(30)]
        public string Fullname { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [Required]
        [MaxLength(15)]
        
        public string PhoneNumber { get; set; }
        [MaxLength(100)]
        public string Address1 { get; set; }
        [MaxLength(100)]
        public string Address2 { get; set; }
        [MaxLength(30)]
        public string City { get; set; }
        [MaxLength(10)]
        public string ZipCode { get; set; }
        [MaxLength(500)]
        public string? Note { get; set; }


    }
}
