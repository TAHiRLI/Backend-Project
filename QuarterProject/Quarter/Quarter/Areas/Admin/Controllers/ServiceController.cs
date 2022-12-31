using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using System.Data;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class ServiceController : Controller
    {
        private readonly QuarterDbContext _context;

        public ServiceController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index(int? page)
        {
            var Services = _context.Services.ToList();
            int pageSize = 5;
            Pagination<Service> paginatedList = new Pagination<Service>();

            ViewBag.Services = paginatedList.GetPagedNames(Services, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Services == null)
                return NotFound();

            return View();
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
            var serive = _context.Services.FirstOrDefault(x => x.Id == id);
            if (serive == null)
                return NotFound();
            _context.Services.Remove(serive);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
