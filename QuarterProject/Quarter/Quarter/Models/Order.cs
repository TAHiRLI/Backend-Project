using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quarter.Models
{
    public class Order:BaseEntity
    {
        [MaxLength(30)]
        public string Fullname { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [MaxLength(100)]
        public string Address1 { get; set; }
        [MaxLength(100)]
        public string? Address2 { get; set; }
        [MaxLength(30)]
        public string City { get; set; }
        [MaxLength(10)]
        public string ZipCode { get; set; }
        [MaxLength(500)]
        public string? Note { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal HousePrice { get; set; }
        [MaxLength(100)]
        public string HouseTitle { get; set; }
        public string? AppUserId { get; set; }
        public int? HouseId { get; set; }
        public House? House { get; set; }
        public AppUser? AppUser { get; set; }
        public bool? OrderStatus { get; set; } = null;



    }
}
