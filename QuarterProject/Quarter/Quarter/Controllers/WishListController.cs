using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quarter.DAL;
using Quarter.Models;

namespace Quarter.Controllers
{
    public class WishListController : Controller
    {
        private readonly QuarterDbContext _context;

        public WishListController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            List<House> Houses = new List<House>();

            AppUser? user = null;
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                user = _context.AppUsers.Include(x => x.WishlistItems).ThenInclude(x => x.House).ThenInclude(x=> x.HouseImages).FirstOrDefault(x => x.UserName == User.Identity.Name);
            }

            if (user != null)
            {
                Houses = user.WishlistItems.Select(x => x.House).ToList();
            }
            else
            {
                List<int> HouseIds = new List<int>();

                var wishlistStr = Request.Cookies["wishlist"];

                if (wishlistStr != null)
                    HouseIds = JsonConvert.DeserializeObject<List<int>>(wishlistStr);
                Houses = _context.Houses.Include(x=> x.HouseImages).Where(x => HouseIds.Contains(x.Id)).ToList();
            }
                return View(Houses);
        }
    }
}
