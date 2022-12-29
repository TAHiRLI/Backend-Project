namespace Quarter.Models
{
    public class WishlistItem:BaseEntity
    {
        public string AppUserId { get; set; }
        public int HouseId { get; set; }
        public AppUser? AppUser { get; set; }
        public House? House { get; set; }
    }
}
