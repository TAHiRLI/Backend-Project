using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Quarter.DAL;
using Quarter.Models;
using System.Data;
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
            var amenities = _context.Amenities;

            int pageSize = 1;
            int pageNumber = (page ?? 1);
            ViewBag.Names = GetPagedNames(page);
            return View( amenities.ToPagedList(pageNumber, pageSize));
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

        protected IPagedList<Amenity> GetPagedNames(int? page)
        {
            // return a 404 if user browses to before the first page
            if (page.HasValue && page < 1)
                return null;

            // retrieve list from database/whereverand
            var listUnpaged = _context.Amenities.ToList();

            // page the list
            const int pageSize = 2;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;

            return listPaged;
        }
    }
}
