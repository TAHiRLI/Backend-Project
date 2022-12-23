using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quarter.Areas.Admin.ViewModels;
using Quarter.DAL;
using Quarter.Models;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OwnerController:Controller
    {
        private readonly QuarterDbContext _context;

        public OwnerController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            var model = _context.Owners.Include(x => x.Houses).ToList();
            return View(model);
        }

      

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(OwnerViewModel owner)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Owner newOwner = new Owner
            {
                Fullname = owner.Fullname,
                Desc = owner.Desc,
                SharePercent = owner.SharePercent,
                
            };

            return Ok(newOwner);

            _context.Owners.Add(newOwner);

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            var city = _context.Cities.FirstOrDefault(x => x.Id == id);
            if (city == null)
                return NotFound();
            return View(city);
        }
        [HttpPost]
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
            return Ok();
        }
    }
}
