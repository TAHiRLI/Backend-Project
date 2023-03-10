using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualBasic;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Hubs;
using Quarter.Migrations;
using Quarter.Models;
using System.Data;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class OrderController:Controller
    {
        private readonly QuarterDbContext _context;
        private readonly IHubContext<RealTimeHub> _hubContext;

        public OrderController(QuarterDbContext context, IHubContext<RealTimeHub> hubContext)
        {
            this._context = context;
            this._hubContext = hubContext;
        }
        public IActionResult Index(int? page)
        {

            int pageSize = 5;
            var Orders = _context.Orders
                .Include(x=> x.House)
                .Include(x=> x.AppUser)
                .OrderByDescending(x=> x.OrderStatus == null)
                .ThenByDescending(x=> x.CreatedAt)
                .ToList();

            Pagination<Order> paginatedList = new Pagination<Order>();
            ViewBag.Orders = paginatedList.GetPagedNames(Orders, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Orders == null)
                return NotFound();


            return View();
        }
        
        public IActionResult Edit(int id)
        {
            var order = _context.Orders.Include(x=> x.House).ThenInclude(x=> x.HouseImages).Include(x=> x.AppUser).FirstOrDefault(x => x.Id == id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        public async Task<IActionResult>  Approve(int id)
        {
            var order = _context.Orders.Include(x=> x.House).Include(x=> x.AppUser).FirstOrDefault(x => x.Id == id);
            if (order == null || order.House == null || order.House.IsSold == true)
                return NotFound();

            if (order.OrderStatus != null)
                return NotFound();

            order.OrderStatus = true;
            order.UpdatedAt = DateTime.UtcNow.AddHours(4);
            order.House.IsSold = true;


            if(order.AppUser?.ConnectionId != null)
            {
                await _hubContext.Clients.Client(order.AppUser?.ConnectionId).SendAsync("OrderAccepted");
            }

            _context.SaveChanges();


         await   _hubContext.Clients.All.SendAsync("updateMonthlyEarning", _context.Orders.Where(x => x.CreatedAt.Month == DateTime.UtcNow.Month && x.OrderStatus==true).Sum(x => x.HousePrice));
         await   _hubContext.Clients.All.SendAsync("updateAnnualEarning", _context.Orders.Where(x => x.CreatedAt.Year == DateTime.UtcNow.Year && x.OrderStatus==true).Sum(x => x.HousePrice));

            return RedirectToAction("index");
        }
        public async Task<IActionResult> Reject(int id)
        {
            var order = _context.Orders.Include(x => x.House).Include(x => x.AppUser).FirstOrDefault(x => x.Id == id);
            if (order == null || order.House == null)
                return NotFound();

            if (order.OrderStatus != null)
                return NotFound();

            order.OrderStatus = false;


            if (order.AppUser?.ConnectionId != null)
            {
                await _hubContext.Clients.Client(order.AppUser?.ConnectionId).SendAsync("OrderRejected");
            }

            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
