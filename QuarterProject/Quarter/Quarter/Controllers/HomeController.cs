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
            Model.Amenities = _context.Amenities.ToList();
            Model.Sliders = _context.HomeSliders.ToList();
            Model.Houses = _context.Houses.Include(x=> x.HouseImages).Include(x=> x.Owner).Include(x=> x.City).Where(x=> x.IsFeatured == true).ToList();
            Model.Cities = _context.Cities.ToList();
            Model.Categories = _context.Categories.ToList();
            Model.Services = _context.Services.ToList();
            Model.Settings = _context.Settings.ToDictionary(x=> x.Key, x=> x.Value);

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