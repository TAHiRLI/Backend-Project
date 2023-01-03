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
        [MaxLength(200)]
        public string? ConnectionId { get; set; }
        public bool IsAdmin { get; set; } = false;
        public DateTime LastConnectedAt { get; set; }
        public DateTime LastRequestedEmail { get; set; } = DateTime.UtcNow.AddHours(4);
        public List<WishlistItem>? WishlistItems { get; set; }  = new List<WishlistItem>();
        public List<UserComment>? Comments { get; set; } = new List<UserComment>();
        public List<Order>? Orders { get; set; } = new List<Order>();



    }
}
