using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Quarter.DAL;
using Quarter.Models;

namespace Quarter.Controllers
{
    public class HouseController : Controller
    {
        private readonly QuarterDbContext _context;

        public HouseController(QuarterDbContext context)
        {
            this._context = context;
        }
       
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetHouseData(int id)
        {
            var house = _context.Houses
                .Include(x=>x.HouseImages)
                .FirstOrDefault(x => x.Id == id);
            return PartialView("_HouseModalPartial", house);
        }
    }
}
