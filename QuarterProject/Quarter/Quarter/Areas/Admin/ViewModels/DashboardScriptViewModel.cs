using Quarter.Models;

namespace Quarter.Areas.Admin.ViewModels
{
    public class DashboardScriptViewModel
    {
        public double HouseSalePercent { get; set; }
        public int[] OrderCountPerMonth { get; set; }
        public int[] CommentCountPerMonth { get; set; }
        public int[] MessageCountPerMonth { get; set; }
        public List<Category> TopOrderedCategories = new List<Category>();
    }
}
