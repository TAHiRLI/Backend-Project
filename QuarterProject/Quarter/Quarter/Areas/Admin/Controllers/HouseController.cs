using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HouseController : Controller
    {
        private readonly QuarterDbContext _context;
        private readonly IWebHostEnvironment _env;

        public HouseController(QuarterDbContext context, IWebHostEnvironment env)
        {
            this._context = context;
            this._env = env;
        }
        public IActionResult Index()
        {
            var model = _context.Houses
                .Include(x => x.Category)
                .Include(x => x.Owner)
                .Include(x => x.City)
                .Include(x => x.HouseImages)
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
            if (!_context.Cities.Any(x => x.Id == house.CityId))
                ModelState.AddModelError("CityId", "City not found");
            if (!_context.Categories.Any(x => x.Id == house.CategoryId))
                ModelState.AddModelError("CategoryId", "Category not found");
            if (!_context.Owners.Any(x => x.Id == house.OwnerId))
                ModelState.AddModelError("OwnerId", "Owner not found");
            if (house.File == null)
                ModelState.AddModelError("File", "Poster Image is required");
            if (house.AmenityIds != null)
            {
                foreach (var amenityId in house.AmenityIds)
                {
                    if (!_context.Amenities.Any(x => x.Id == amenityId))
                    {
                        ModelState.AddModelError("AmenityIds", "Amenity not found");
                        break;
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = _context.Cities.ToList();
                ViewBag.Amenities = _context.Amenities.ToList();
                ViewBag.Owners = _context.Owners.ToList();
                ViewBag.Categories = _context.Categories.ToList();
                return View();
            }


            HouseImage posterImage = new HouseImage
            {
                ImageUrl = FileManager.Save(house.File, _env.WebRootPath, "Uploads/Houses", 100),
                PosterStatus = true,
            };
            house.HouseImages.Add(posterImage);

            if (house.Files != null)
            {
                foreach (var file in house.Files)
                {
                    HouseImage OtherImage = new HouseImage
                    {
                        ImageUrl = FileManager.Save(file, _env.WebRootPath, "Uploads/Houses", 100),
                        PosterStatus = false,
                    };
                    house.HouseImages.Add(OtherImage);
                }

            }

            house.CreatedAt = DateTime.UtcNow.AddHours(4);
            house.UpdatedAt = DateTime.UtcNow.AddHours(4);

            _context.Houses.Add(house);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
