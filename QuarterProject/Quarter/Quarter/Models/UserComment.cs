using System.ComponentModel.DataAnnotations;

namespace Quarter.Models
{
    public class UserComment
    {
        public int Id { get; set; }
        public int? HouseId { get; set; }
        public string? AppUserId{ get; set; }
        [MaxLength(500)]
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public House? House { get; set; }
        public  AppUser? AppUser { get; set; }
        public bool IsApproved { get; set; }
    }
}
