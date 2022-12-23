using Microsoft.AspNetCore.Mvc;
using Quarter.DAL;

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
    }
}
