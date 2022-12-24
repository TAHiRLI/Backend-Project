using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Project;
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

            foreach (var Id in house.AmenityIds)
            {
                HouseAmenity houseAmenity = new HouseAmenity
                {
                    AmenityId = Id
                };
                house.HouseAmenities.Add(houseAmenity);
            }

            house.CreatedAt = DateTime.UtcNow.AddHours(4);
            house.UpdatedAt = DateTime.UtcNow.AddHours(4);

            _context.Houses.Add(house);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit (int id)
        {
            var model = _context.Houses
                .Include(x=> x.HouseAmenities)
                .Include(x=> x.City)
                .Include(x=> x.Category)
                .Include(x=> x.Owner)
                .Include(x=> x.HouseImages)
                .FirstOrDefault(h => h.Id == id);
            if(model == null)
                return NotFound();


            ViewBag.Cities = _context.Cities.ToList();
            ViewBag.Amenities = _context.Amenities.ToList();
            ViewBag.Owners = _context.Owners.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            model.AmenityIds = model.HouseAmenities.Select(x => x.AmenityId).ToList();
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(House house)
        {
            var existHouse = _context.Houses.Include(x=> x.HouseImages).Include(x=> x.HouseAmenities).FirstOrDefault(x => x.Id == house.Id);
            if (existHouse == null)
                return NotFound();

            if (!_context.Cities.Any(x => x.Id == house.CityId))
                ModelState.AddModelError("CityId", "City not found");
            if (!_context.Categories.Any(x => x.Id == house.CategoryId))
                ModelState.AddModelError("CategoryId", "Category not found");
            if (!_context.Owners.Any(x => x.Id == house.OwnerId))
                ModelState.AddModelError("OwnerId", "Owner not found");
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = _context.Cities.ToList();
                ViewBag.Amenities = _context.Amenities.ToList();
                ViewBag.Owners = _context.Owners.ToList();
                ViewBag.Categories = _context.Categories.ToList();
                return View();
            }
           


            existHouse.HouseAmenities.RemoveAll(x => !house.AmenityIds.Contains(x.AmenityId));

            foreach (var amenityId in house.AmenityIds.Where(x=> !existHouse.HouseAmenities.Any(a=> a.AmenityId == x) ).ToList())
            {
                HouseAmenity newHouseAmenity = new HouseAmenity
                {
                    AmenityId = amenityId,
                };
                existHouse.HouseAmenities.Add(newHouseAmenity);
            }


            if(house.File != null)
            {
                var posterImg = existHouse.HouseImages.FirstOrDefault(x => x.PosterStatus == true)?.ImageUrl;
                if (posterImg == null)
                    return NotFound();
                FileManager.Delete(_env.WebRootPath, "Uploads/Houses", posterImg);
                existHouse.HouseImages.FirstOrDefault(x=> x.PosterStatus).ImageUrl = FileManager.Save(house.File, _env.WebRootPath, "Uploads/Houses", 100);
            }


            var removedImages = existHouse.HouseImages.Where(x => !house.HouseImgIds.Contains(x.Id) && x.PosterStatus == false).ToList();

            foreach (var image in removedImages)
            {
                FileManager.Delete(_env.WebRootPath, "Uploads/Books", image.ImageUrl);
                existHouse.HouseImages.Remove(image);
            }

            if (house.Files != null)
            {
                foreach (var file in house.Files)
                {
                    HouseImage OtherImage = new HouseImage
                    {
                        ImageUrl = FileManager.Save(file, _env.WebRootPath, "Uploads/Houses", 100),
                        PosterStatus = false,
                    };
                    existHouse.HouseImages.Add(OtherImage);
                }

            }




            existHouse.Title = house.Title;
            existHouse.Desc = house.Desc;
            existHouse.Location = house.Location;
            existHouse.CategoryId = house.CategoryId;
            existHouse.OwnerId = house.OwnerId;
            existHouse.CityId = house.CityId;
            existHouse.Area = house.Area;
            existHouse.RoomCount = house.RoomCount;
            existHouse.BathroomCount = house.BathroomCount;
            existHouse.ParkingCount = house.ParkingCount; ;
            existHouse.BedroomCount = house.BedroomCount;
            existHouse.DiscountPercent = house.DiscountPercent;
            existHouse.Price = house.Price;
            existHouse.PosterDesc = house.PosterDesc;
            existHouse.IsFeatured = house.IsFeatured;
            existHouse.IsSold = house.IsSold;
            existHouse.UpdatedAt = DateTime.UtcNow.AddHours(4);

            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
