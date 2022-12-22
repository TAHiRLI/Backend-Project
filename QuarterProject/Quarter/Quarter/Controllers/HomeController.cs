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
            Model.Houses = _context.Houses.Include(x=> x.HouseImages).Include(x=> x.Owner).ToList();


            return View(Model);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}