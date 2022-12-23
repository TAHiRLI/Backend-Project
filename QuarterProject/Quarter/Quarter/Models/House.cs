using Quarter.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quarter.Models
{
    public class House:BaseEntity
    {
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Location { get; set; }
        [MaxLength(1000)]
        public string Desc { get; set; }
        [MaxLength(100)]
        public string? PosterDesc { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal Area { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public byte RoomCount { get; set; }
        public byte BedroomCount { get; set; }
        public byte BathroomCount { get; set; }
        public byte ParkingCount { get; set; }
        public int YearBuilt { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercent { get; set; }
        public bool IsSold { get; set; }
        public bool IsFeatured { get; set; }
        //foreign keys
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }

        public Category? Category { get; set; }
        public Owner? Owner { get; set; }
        [NotMapped]
        public List<int>? AmenityIds { get; set; }
        [NotMapped]
        [AllowedFileTypes("image/jpeg", "image/png")]
        [MaxFileSize(2)]
        public IFormFile? File { get; set; }
        [NotMapped]
        [AllowedFileTypes("image/jpeg", "image/png")]
        [MaxFileSize(2)]
        public List<IFormFile>? Files { get; set; }


        public List<HouseImage>? HouseImages { get; set; } 
        public List<HouseAmenity>? HouseAmenities { get; set; }




    }
}
