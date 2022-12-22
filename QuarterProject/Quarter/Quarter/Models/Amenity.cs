using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Quarter.Models
{
    public class Amenity
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string Icon { get; set; }
        public bool IsFeatured { get; set; }
        public List<HouseAmenity>? HouseAmenities { get; set; }
    }
}
