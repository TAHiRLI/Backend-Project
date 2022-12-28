using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;

namespace Quarter.Models
{
    public class UserBookingMessage:BaseEntity
    {
        [MaxLength(30)]
        public  string Fullname { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }

        public string AppUserId { get; set; }
        public int HouseId { get; set; }
        public AppUser? AppUser { get; set; }
        public House? House { get; set; }
    }
}
