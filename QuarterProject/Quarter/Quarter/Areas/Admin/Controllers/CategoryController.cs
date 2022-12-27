using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using System.Data;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class CategoryController : Controller
    {
        private readonly QuarterDbContext _context;

        public CategoryController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index(int? page)
        {
            int pageSize = 5;
            var model = _context.Categories.Include(x => x.Houses).ToList();

            Pagination<Category> paginatedList = new Pagination<Category>();

            ViewBag.Categories = paginatedList.GetPagedNames(model, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Categories == null)
                return NotFound();


            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existCategory = _context.Categories.FirstOrDefault(x => x.Id == category.Id);
            if (existCategory == null)
                return NotFound();
            existCategory.Name = category.Name;
            _context.SaveChanges();


            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
