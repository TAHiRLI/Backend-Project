using Microsoft.AspNetCore.Mvc;
using Quarter.DAL;
using Quarter.Models;
using System.Diagnostics;

namespace Quarter.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuarterDbContext _context;

        public HomeController(QuarterDbContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}