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
    public class SliderController : Controller
    {
        private readonly QuarterDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(QuarterDbContext context,IWebHostEnvironment env)
        {
            this._context = context;
            this._env = env;
        }
        public IActionResult Index(int? page)
        {
            var Sliders = _context.HomeSliders.OrderBy(x=> x.Order).ToList();


            int pageSize = 5;
            Pagination<HomeSlider> paginatedList = new Pagination<HomeSlider>();

            ViewBag.Sliders = paginatedList.GetPagedNames(Sliders, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Sliders == null)
                return NotFound();

            return View();
        }
        public IActionResult Create()
        {
            ViewBag.LastOrder = _context.HomeSliders.Max(x => x.Order) + 1;
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]

        public IActionResult Create(HomeSlider slider)
        {
            if (slider.File == null)
                ModelState.AddModelError("File", "This field is required");
            if (slider.Order > _context.HomeSliders.Max(x => x.Order) + 1)
                ModelState.AddModelError("Order", "Order number cannot be higher than total number of sliders");
            if (!ModelState.IsValid)
            {
                return View();
            }
            foreach (var existSlide in _context.HomeSliders.Where(x=> x.Order >= slider.Order).ToList())
            {
                existSlide.Order++;
            }

            slider.ImageUrl = FileManager.Save(slider.File, _env.WebRootPath, "Uploads/Sliders", 100);


            _context.HomeSliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            var slider = _context.HomeSliders.FirstOrDefault(x => x.Id == id);
            if (slider == null)
                return NotFound();

            ViewBag.LastOrder = slider.Order;
            return View(slider);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]

        public IActionResult Edit(HomeSlider slider)
        {
            if (!ModelState.IsValid)
            {
                return View(slider);
            }
            var existSlider = _context.HomeSliders.FirstOrDefault(x => x.Id == slider.Id);
            if (existSlider == null)
                return NotFound();


            var counter = slider.Order;
            foreach (var existSlide in _context.HomeSliders.Where(x => x.Order >= slider.Order).ToList())
            {
                counter++;
                existSlide.Order = counter;
            }
          

            if (slider.File != null)
            {
                FileManager.Delete(_env.WebRootPath, "Uploads/Sliders", existSlider.ImageUrl);
                existSlider.ImageUrl = FileManager.Save(slider.File, _env.WebRootPath, "Uploads/Sliders", 100);
            }




            existSlider.Title = slider.Title;
            existSlider.Desc = slider.Desc;
            existSlider.Order = slider.Order;


            _context.SaveChanges();


            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            var slider = _context.HomeSliders.FirstOrDefault(x => x.Id == id);
            if (slider == null)
                return NotFound();

            FileManager.Delete(_env.WebRootPath, "Uplodas/Sliders", slider.ImageUrl);
            _context.HomeSliders.Remove(slider);

            //
            var counter = slider.Order;
            foreach (var existSlide in _context.HomeSliders.Where(x => x.Order >= slider.Order).ToList())
            {
                existSlide.Order--;
            }


            _context.SaveChanges();
            return RedirectToAction("index");
        }

    }
}
