using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Quarter.DAL;
using Quarter.Models;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly QuarterDbContext _context;

        public ServiceController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            var model = _context.Services.ToList();
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Service service)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Services.Add(service);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            var service = _context.Services.FirstOrDefault(x => x.Id == id);
            if (service == null)
                return NotFound();
            return View(service);
        }
        [HttpPost]
        public IActionResult Edit(Service service)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existService = _context.Services.FirstOrDefault(x => x.Id == service.Id);
            if (existService == null)
                return NotFound();
            existService.Name = service.Name;
            existService.Desc = service.Desc;
            existService.IsFeatured = service.IsFeatured;
            existService.Icon = service.Icon;
            _context.SaveChanges();


            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            
            return Ok();
        }
    }
}
