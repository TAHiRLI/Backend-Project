using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using Quarter.Areas.Admin.ViewModels;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using System.Data;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class OwnerController:Controller
    {
        private readonly QuarterDbContext _context;

        public readonly IWebHostEnvironment _env;

        public OwnerController(QuarterDbContext context, IWebHostEnvironment env)
        {
            this._context = context;
            _env = env;
        }
        public IActionResult Index(int? page)
        {
            var Owners = _context.Owners.Include(x => x.Houses).ToList();


            int pageSize = 5;
            Pagination<Owner> paginatedList = new Pagination<Owner>();

            ViewBag.Owners = paginatedList.GetPagedNames(Owners, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Owners == null)
                return NotFound();

            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]  

        public IActionResult Create(Owner owner)
        {
            if (owner.File == null)
                ModelState.AddModelError("File", "This field is required");
            if (!ModelState.IsValid)
            {
                return View();
            }
            owner.ImageUrl = FileManager.Save(owner.File, _env.WebRootPath, "Uploads/Owners", 100);
            _context.Owners.Add(owner);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            var owner = _context.Owners.FirstOrDefault(x => x.Id == id);
            if (owner == null)
                return NotFound();
            return View(owner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(Owner owner)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existOwner = _context.Owners.FirstOrDefault(x => x.Id == owner.Id);
            if (existOwner == null)
                return NotFound();


            if(owner.File != null)
            {
                FileManager.Delete(_env.WebRootPath, "Uploads/Owners", existOwner.ImageUrl);
                existOwner.ImageUrl = FileManager.Save(owner.File, _env.WebRootPath, "Uploads/Owners", 100);
            }




            existOwner.Fullname = owner.Fullname;
            existOwner.Desc = owner.Desc;
            existOwner.SharePercent = owner.SharePercent;

            
            _context.SaveChanges();


            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            var owner = _context.Owners.FirstOrDefault(x => x.Id == id);
            if (owner == null)
                return NotFound();

            FileManager.Delete(_env.WebRootPath, "Uploads/Owners", owner.ImageUrl);

            _context.Owners.Remove(owner);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
