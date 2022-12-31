using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Quarter.DAL;
using Quarter.Models;
using Quarter.ViewModels;

namespace Quarter.Controllers
{
    public class OrderController : Controller
    {
        private readonly QuarterDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(QuarterDbContext context, UserManager<AppUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }
        public async Task<IActionResult > Checkout(int HouseId)
        {
            var house = _context.Houses.Include(x=> x.Owner).FirstOrDefault(x => x.Id == HouseId);
            if(house == null) return NotFound();
           

            Order order = new Order();
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                order.AppUser = await _userManager.FindByNameAsync(User.Identity.Name);
                order.AppUserId = order.AppUser.Id;
            }

            order.House = house;
            order.HouseId = house.Id;
            order.HousePrice = house.Price;

            return View(order);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            if (order.HouseId == null)
                return NotFound();
            if (!ModelState.IsValid)
            {
                var house = _context.Houses.Include(x => x.Owner).FirstOrDefault(x => x.Id == order.HouseId);
                if (house == null) return NotFound();

                return View(order);
            }

            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                order.AppUser = await _userManager.FindByNameAsync(User.Identity.Name);
                order.AppUserId = order.AppUser.Id;
            }
            _context.Orders.Add(order);
            _context.SaveChanges();

            return RedirectToAction("index", "home");
        }
    }
}
