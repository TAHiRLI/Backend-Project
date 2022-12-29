using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace Quarter.Models
{
    public class AppUser : IdentityUser
    {
        [MaxLength(30)]
        public string Fullname { get; set; }
        public bool IsSubscribed { get; set; }
        [MaxLength(100)]
        public string UserPhoto { get; set; } = "defaultUser.jpeg";
        public List<WishlistItem>? WishlistItems { get; set; }  = new List<WishlistItem>();


    }
}
