using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quarter.Models
{
    public class House:BaseEntity
    {
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(200)]
        public string Location { get; set; }
        [MaxLength(500)]
        public string Desc { get; set; }
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
        [Required]
        public int CityId { get; set; }

        public Category? Category { get; set; }
        public Owner? Owner { get; set; }
        public City? City { get; set; }


        public List<HouseImage>? HouseImages { get; set; } 
        public List<HouseAmenity>? HouseAmenities { get; set; }




    }
}
