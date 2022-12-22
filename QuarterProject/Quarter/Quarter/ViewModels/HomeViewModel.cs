using Quarter.Models;

namespace Quarter.ViewModels
{
    public class HomeViewModel
    {
        public List<House> Houses { get; set; } = new List<House>();
        public List<Amenity> Amenities { get; set; }= new List<Amenity>();
        public List<HomeSlider> Sliders { get; set; }= new List<HomeSlider>();
    }
}
