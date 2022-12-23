using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quarter.DAL;
using Quarter.Models;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HouseController : Controller
    {
        private readonly QuarterDbContext _context;

        public HouseController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            var model = _context.Houses
                .Include(x=> x.Category)
                .Include(x=> x.Owner)
                .Include(x=> x.City)
                .Include(x=> x.HouseImages)
                .ToList();
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Cities = _context.Cities.ToList();
            ViewBag.Amenities = _context.Amenities.ToList();
            ViewBag.Owners = _context.Owners.ToList();
            ViewBag.Categories = _context.Categories.ToList();

            return View();
        }
        [HttpPost]
        public IActionResult Create(House house)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Cities = _context.Cities.ToList();
            //    ViewBag.Amenities = _context.Amenities.ToList();
            //    ViewBag.Owners = _context.Owners.ToList();
            //    ViewBag.Categories = _context.Categories.ToList();
            //    return View();
            //}
            return Ok(house);
        }
    }
}
