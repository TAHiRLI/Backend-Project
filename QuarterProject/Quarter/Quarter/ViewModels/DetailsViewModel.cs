using Quarter.Models;

namespace Quarter.ViewModels
{
    public class DetailsViewModel
    {
        public House House { get; set; }
        public List<House> RelatedProducts { get; set; }
        public List<Category> TopCategories { get; set; }
    }
}
