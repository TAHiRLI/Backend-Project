using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using System.Data;
using System.Net;
using X.PagedList;

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
        public IActionResult Index(int? page =1 )
        {

            int pageSize = 5;
            var Amenities = _context.Amenities.ToList();
            Pagination<Amenity> paginatedList = new Pagination<Amenity>();

            ViewBag.Amenities = paginatedList.GetPagedNames(Amenities,page, pageSize);
            ViewBag.PageNumber =(page ?? 1) ;
            ViewBag.PageSize =pageSize ;
            if (ViewBag.Amenities == null)
                return NotFound();


            return View();
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
            var amenity = _context.Amenities.FirstOrDefault(x => x.Id == id);
            if (amenity == null)
                return NotFound();
            
            _context.Amenities.Remove(amenity);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

      
    }
}
