using Microsoft.AspNetCore.Mvc;
using Quarter.DAL;
using Quarter.Models;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : Controller
    {
        private readonly QuarterDbContext _context;

        public SettingController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            var model = _context.Settings.ToList();
            if (model == null)
                return NotFound();

            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Setting setting)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _context.Settings.Add(setting);
            _context.SaveChanges();

            return RedirectToAction("index");
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
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existSetting = _context.Settings.FirstOrDefault(x => x.Id == setting.Id);
            if (existSetting == null)
                return NotFound();

            existSetting.Key = setting.Key;
            existSetting.Value = setting.Value;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            var setting = _context.Settings.FirstOrDefault(x => x.Id == id);
            if (setting == null)
                return NotFound();

            _context.Settings.Remove(setting);
            _context.SaveChanges();

            return Ok();
        }
    }
}
