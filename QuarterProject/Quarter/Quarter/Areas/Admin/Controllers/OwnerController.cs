using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return Ok();
        }
    }
}
