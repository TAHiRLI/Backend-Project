using NuGet.Protocol.Core.Types;
using Quarter.Models;

namespace Quarter.ViewModels
{
    public class ShopViewModel
    {
        public List<City> Cities { get; set; } = new List<City>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Amenity> Amenities { get; set; } = new List<Amenity>();
        public List<House>Houses { get; set; } = new List<House>();
        public decimal MaxPrice { get; set; } 
        public decimal MinPrice { get; set; }

    }
}
