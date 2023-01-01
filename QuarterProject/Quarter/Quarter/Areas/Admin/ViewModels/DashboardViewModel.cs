using Microsoft.Build.ObjectModelRemoting;
using NuGet.Protocol.Core.Types;
using Quarter.Models;
using System.Security.Policy;

namespace Quarter.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        public AppUser Admin { get; set; }

       
        public  List<House> Houses { get; set; } = new List<House>();
        public  List<Category> Categories { get; set; } = new List<Category>();
        public  List<Order> Orders { get; set; } = new List<Order>();
        public  List<City> Cities { get; set; } = new List<City>();
        public List<Owner> Owners { get; set; } = new List<Owner>();
        public List<Amenity> Amenities { get; set; } = new List<Amenity>();
        public DashboardScriptViewModel DashboardScriptVm { get; set; } = new DashboardScriptViewModel();
    }
}
