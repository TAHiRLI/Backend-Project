using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quarter.DAL;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HouseController : Controller
    {
        private readonly QuarterDbContext _context;

        public HouseController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            var model = _context.Houses
                .Include(x=> x.Category)
                .Include(x=> x.Owner)
                .Include(x=> x.City)
                .Include(x=> x.HouseImages)
                .ToList();
            return View(model);
        }
    }
}
