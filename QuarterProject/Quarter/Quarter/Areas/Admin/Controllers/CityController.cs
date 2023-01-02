using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using System.Data;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class CityController : Controller
    {
        private readonly QuarterDbContext _context;

        public CityController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index(int? page)
        {


            int pageSize = 5;
            
            var Cities = _context.Cities.Include(x => x.Houses).ToList();
            Pagination<City> paginatedList = new Pagination<City>();

            ViewBag.Cities = paginatedList.GetPagedNames(Cities, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Cities == null)
                return NotFound();

            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(City city)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Cities.Add(city);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            var city = _context.Cities.FirstOrDefault(x=> x.Id == id);
            if (city == null)
                return NotFound();
            return View(city);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(City city)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existCity = _context.Cities.FirstOrDefault(x => x.Id == city.Id);
            if (existCity == null)
                return NotFound();
            existCity.Name = city.Name;
            _context.SaveChanges();


            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            var city = _context.Cities.FirstOrDefault(x => x.Id == id);
            if (city == null)
                return NotFound();
            _context.Cities.Remove(city);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
