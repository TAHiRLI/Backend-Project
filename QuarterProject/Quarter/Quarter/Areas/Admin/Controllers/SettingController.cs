using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using System.Data;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class SettingController : Controller
    {
        private readonly QuarterDbContext _context;

        public SettingController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index(int? page)
        {
            var Settings = _context.Settings.ToList();
            int pageSize = 5;
            Pagination<Setting> paginatedList = new Pagination<Setting>();

            ViewBag.Settings = paginatedList.GetPagedNames(Settings, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Settings == null)
                return NotFound();


            return View();
        }
   

        public IActionResult Edit(int id)
        {
            var setting = _context.Settings.FirstOrDefault(x => x.Id == id);
            if (setting == null)
                return NotFound();
            return View(setting);
        }
        [HttpPost]
        public IActionResult Edit(Setting setting)
        {
            if (setting.Value == null)
                ModelState.AddModelError("Value", "This field is required");
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existSetting = _context.Settings.FirstOrDefault(x => x.Id == setting.Id);
            if (existSetting == null)
                return NotFound();

            existSetting.Value = setting.Value;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

       
    }
}
