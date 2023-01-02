using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quarter.Areas.Admin.ViewModels;
using Quarter.DAL;
using Quarter.Models;
using System.Drawing;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="SuperAdmin, Admin")]
    public class DashboardController : BaseController
    {
        private readonly QuarterDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(QuarterDbContext context,UserManager<AppUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            AppUser admin = await _userManager.FindByNameAsync(UserName);
            if (admin == null)
                return NotFound();

            DashboardViewModel DashboardVm = new DashboardViewModel();
            var HouseQuery = _context.Houses
                .Include(x => x.UserComments)
                .Include(x => x.UserBookingMessages)
                .AsQueryable();

            DashboardVm.Admin = admin;
            DashboardVm.Categories = _context.Categories.Include(x => x.Houses).ToList();
            DashboardVm.Cities = _context.Cities.Include(x => x.Houses).ToList();
            DashboardVm.CityTotalVms = _context.Cities.Select(x => new CityTotalViewModel
            {
                CityName = x.Name,
                Total = x.Houses.Sum(x => x.Order.HousePrice)

            }).Take(5).ToList();

            DashboardVm.Orders = _context.Orders
                .Include(x=> x.House).ThenInclude(x=> x.Category)
                .Include(x=> x.House).ThenInclude(x=> x.City)
                .ToList();


      

            DashboardVm.Houses = HouseQuery.ToList();

            // calculate sold house percentage
            int houseCount = HouseQuery.Count();
            int soldHouseCount = HouseQuery.Count(x=> x.IsSold);
      
            DashboardVm.DashboardScriptVm.HouseSalePercent = (double)soldHouseCount / houseCount * 100;
            DashboardVm.DashboardScriptVm.OrderCountPerMonth = GetOrderCountPerMonth();
            DashboardVm.DashboardScriptVm.CommentCountPerMonth = GetCommentCountPerMonth();
            DashboardVm.DashboardScriptVm.MessageCountPerMonth = GetMessageCountPerMonth();

            // get top ordered category
            DashboardVm.DashboardScriptVm.TopOrderedCategories = _context.Categories
                .Include(x=> x.Houses)
                .OrderByDescending(x => x.Houses.Select(x => x.Order.HousePrice).Sum())
                .Take(4)
                .ToList();
          

            return View(DashboardVm);
        }

        private int[] GetOrderCountPerMonth()
        {
            int[] ordersPerMonth = new int[12];

            var currentYear = DateTime.UtcNow.Year;
           
            for (int i = 1; i <=ordersPerMonth.Length; i++)
            {
                var MontylyOrderCount = _context.Orders.Count(x => x.CreatedAt.Month == i && x.CreatedAt.Year == currentYear);
                ordersPerMonth[i - 1] = MontylyOrderCount;
            }

            return ordersPerMonth;
        }
        private int[] GetCommentCountPerMonth()
        {
            int[] commentsPerMonth = new int[12];

            var currentYear = DateTime.UtcNow.Year;

            for (int i = 1; i <= commentsPerMonth.Length; i++)
            {
                var MontylyOrderCount = _context.UserComments.Count(x => x.CreatedAt.Month == i && x.CreatedAt.Year == currentYear);
                commentsPerMonth[i - 1] = MontylyOrderCount;
            }

            return commentsPerMonth;
        }
        private int[] GetMessageCountPerMonth()
        {
            int[] messagesPerMonth = new int[12];

            var currentYear = DateTime.UtcNow.Year;

            for (int i = 1; i <= messagesPerMonth.Length; i++)
            {
                var MontylyMessageCount = _context.UserBookingMessages.Count(x => x.CreatedAt.Month == i && x.CreatedAt.Year == currentYear);
                messagesPerMonth[i - 1] = MontylyMessageCount;
            }

            return messagesPerMonth;
        }


    }
}


