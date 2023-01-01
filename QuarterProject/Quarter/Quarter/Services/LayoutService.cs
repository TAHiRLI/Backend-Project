
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Quarter.DAL;
using Quarter.Models;

namespace Quarter.Services
{
    public class LayoutService
    {
        private readonly QuarterDbContext _context;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(QuarterDbContext context, IHttpContextAccessor httpContext, UserManager<AppUser> userManager )
        {
            this._context = context;
            this._httpAccessor = httpContext;
            this._userManager = userManager;
        }
        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);

        }
        public async Task<List<House>> GetWishList()
        {
            List<House> Houses = new List<House>();
            
            AppUser? user = null;
            if(_httpAccessor.HttpContext.User.Identity.IsAuthenticated && _httpAccessor.HttpContext.User.IsInRole("Member"))
            {
                var username =  _httpAccessor.HttpContext.User.Identity.Name;
                user = _context.AppUsers.Include(x => x.WishlistItems).ThenInclude(x=> x.House).FirstOrDefault(b => b.UserName == username);
            }
            if(user != null)
            {
                Houses = user.WishlistItems.Select(x=> x.House).ToList();
            }
            else
            {
                var wishlistStr = _httpAccessor.HttpContext.Request.Cookies["wishlist"];
                List<int> HouseIds = new List<int>();

                if (wishlistStr != null) 
                   HouseIds = JsonConvert.DeserializeObject<List<int>>(wishlistStr);

                Houses = _context.Houses.Where(x => HouseIds.Contains(x.Id)).ToList();
            }



            return Houses;
        }

        //=================================
        //Admin related services
        //=================================

        public int GetPendingOrderCount()
        {
            int count = 0;
            count = _context.Orders.Count(x => x.OrderStatus == null);
            return count;
        }
        public int GetPendingCommentCount()
        {
            int count = 0;
            count = _context.UserComments.Count(x => x.IsApproved == false);
            return count;   
        }
        public int GetPendingBookingRequestCount()
        {
            int count = 0;
            count = _context.UserBookingMessages.Count(x => x.IsReplied == false);
            return count;
        }


    }
}
