using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quarter.DAL;
using Quarter.Models;
using Quarter.ViewModels;
using System.Diagnostics;

namespace Quarter.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuarterDbContext _context;

        public HomeController(QuarterDbContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel Model = new HomeViewModel();
            Model.Cities = _context.Cities.ToList();
            Model.Services = _context.Services.ToList();
            Model.Amenities = _context.Amenities.ToList();
            Model.Categories = _context.Categories.ToList();
            Model.Sliders = _context.HomeSliders.OrderBy(x=> x.Order).ToList();
            Model.Settings = _context.Settings.ToDictionary(x=> x.Key, x=> x.Value);

            Model.Houses = _context.Houses
                .Include(x=> x.HouseImages)
                .Include(x=> x.Owner)
                .Include(x=> x.City)
                .Where(x => x.IsFeatured)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToList();

            Model.Comments = _context.UserComments
                .Include(x=> x.AppUser)
                .Where(x=> x.IsFeatured == true)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToList();

            Model.LastOrderedHouse =  _context.Houses
                 .Include(x => x.Order)
                 .Include(x => x.HouseImages)
                 .Include(x => x.HouseAmenities).ThenInclude(x=> x.Amenity)
                 .Where(x => x.Order.OrderStatus == true)
                 .OrderByDescending(x => x.Order.CreatedAt)
                 .Take(1).FirstOrDefault();

            Model.TotalArea = _context.Houses.Sum(x => x.Area);
            Model.TotalHouseCount = _context.Houses.Count();
            Model.TotalOrderCount = _context.Orders.Count(x=> x.OrderStatus == true);
            Model.TotalPrice = _context.Orders.Where(x=> x.OrderStatus == true).Sum(x => x.HousePrice);


            ViewBag.Cities = Model.Cities;
            ViewBag.Categories = Model.Categories;

            return View(Model);
        }

        public IActionResult Error()
        {
            return View();
        }
      
    }
}