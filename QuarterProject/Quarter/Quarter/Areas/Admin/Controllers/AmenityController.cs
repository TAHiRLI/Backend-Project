using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quarter.DAL;
using Quarter.Models;
using System.Data;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AmenityController : Controller
    {
        private readonly QuarterDbContext _context;

        public AmenityController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            var model = _context.Amenities.ToList();
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Amenity amenity)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Amenities.Add(amenity);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            var amenity = _context.Amenities.FirstOrDefault(x => x.Id == id);
            if (amenity == null)
                return NotFound();
            return View(amenity);
        }
        [HttpPost]
        public IActionResult Edit(Amenity amenity)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existService = _context.Amenities.FirstOrDefault(x => x.Id == amenity.Id);
            if (existService == null)
                return NotFound();

            existService.Name = amenity.Name;
            existService.IsFeatured = amenity.IsFeatured;
            existService.Icon = amenity.Icon;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {

            return Ok();
        }
    }
}
