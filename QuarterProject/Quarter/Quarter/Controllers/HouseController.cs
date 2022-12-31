using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Quarter.DAL;
using Quarter.Models;
using Quarter.ViewModels;
using System.Security.Claims;

namespace Quarter.Controllers
{
    public class HouseController : Controller
    {
        private readonly QuarterDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HouseController(QuarterDbContext context, UserManager<AppUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }
       
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id )
        {
            var house = _context.Houses
                .Include(x=> x.HouseImages)
                .Include(x=> x.HouseAmenities).ThenInclude(x=> x.Amenity)
                .Include(x=> x.Owner)
                .Include(x=> x.City)
                .Include(x=> x.Category)
                .Include(x=> x.UserComments).ThenInclude(x=> x.AppUser)
                .FirstOrDefault(x => x.Id == id);

            if (house == null)
                return NotFound();


            DetailsViewModel DetailsVm = new DetailsViewModel();
            DetailsVm.House = house;
            DetailsVm.RelatedProducts = _context.Houses
                .Include(x=> x.City)
                .Include(x=> x.Category)
                .Include(x=> x.HouseImages)
                .Include(x=> x.UserComments).ThenInclude(x=> x.AppUser)
                .Where(x => x.OwnerId == house.OwnerId && x.Id != house.Id).ToList();

            DetailsVm.CommentFormVm = new CommentViewModel
            {
                HouseId = house.Id
            };

           DetailsVm.TopCategories = _context.Categories.Include(x=> x.Houses).OrderByDescending(x=> x.Houses.Count).Take(5).ToList();
            DetailsVm.UserBookingMessageVm.HouseId = house.Id;
            return View(DetailsVm);
        }

        //================================
        // Get partialview
        //================================
        public IActionResult GetHouseData(int id)
        {
            var house = _context.Houses
                .Include(x=>x.HouseImages)
                .FirstOrDefault(x => x.Id == id);
            return PartialView("_HouseModalPartial", house);
        }
        public IActionResult GetSearchRecommendation(string search)
        {
            var model = _context.Houses
                .Include(x=> x.HouseImages)
                .Where(x => x.Title.Contains(search)).Take(5).ToList();
            return PartialView("_SearchRecommendPartial", model);
        }

        //================================
        // Comment to House
        //================================

        [HttpPost]
        [Authorize(Roles = "Member")]
        public IActionResult Comment(CommentViewModel CommentVm)
        {
            var house = _context.Houses
                .Include(x => x.HouseImages)
                .Include(x => x.HouseAmenities).ThenInclude(x => x.Amenity)
                .Include(x => x.Owner)
                .Include(x => x.City)
                .Include(x => x.Category)
                .Include(x => x.UserComments).ThenInclude(x => x.AppUser)
                .FirstOrDefault(x => x.Id == CommentVm.HouseId);

            if (house == null)
                return NotFound();


            if (!ModelState.IsValid)
            {
                DetailsViewModel DetailsVm = new DetailsViewModel();
                DetailsVm.House = house;
                DetailsVm.RelatedProducts = _context.Houses
               .Include(x => x.City)
               .Include(x => x.Category)
               .Include(x => x.HouseImages)
               .Include(x => x.UserComments).ThenInclude(x => x.AppUser)
               .Where(x => x.OwnerId == house.OwnerId && x.Id != house.Id).ToList();

                DetailsVm.CommentFormVm = CommentVm;
                DetailsVm.TopCategories = _context.Categories.Include(x => x.Houses).OrderByDescending(x => x.Houses.Count).Take(5).ToList();
                return View("details", DetailsVm);
            }
            UserComment userComment = new UserComment
            {
                Text = CommentVm.Text,
                AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.UtcNow.AddHours(4),
                IsApproved = false
            };

            house.UserComments.Add(userComment);
            _context.SaveChanges();

            //^ signalr adminpanel UserMessageVm

            return RedirectToAction("details", new { id = house.Id });

        }
        public IActionResult GetComment(int houseId,int count = 3, int skipCount = 3)
        {
            // javascript fetch is used to load more comments
            var comments = _context.UserComments.Include(x=> x.AppUser).Where(x=> x.HouseId == houseId).Skip(skipCount).Take(count).ToList();   
            return PartialView("_CommentsPartial", comments );
        }
        [Authorize(Roles ="Member")]

        //================================
        // Book a house
        //================================
        public async Task<IActionResult>  BookHouse(UserBookingMessageViewModel UserMessageVm)
        {
            // user sends a message to the admin for ordering book

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NotFound();

            var house = _context.Houses
             .Include(x => x.HouseImages)
             .Include(x => x.HouseAmenities).ThenInclude(x => x.Amenity)
             .Include(x => x.Owner)
             .Include(x => x.City)
             .Include(x => x.Category)
             .Include(x => x.UserComments).ThenInclude(x => x.AppUser)
             .FirstOrDefault(x => x.Id == UserMessageVm.HouseId);

            if (house == null)
                return NotFound();

            if (!ModelState.IsValid)
            {

                DetailsViewModel DetailsVm = new DetailsViewModel();
                DetailsVm.House = house;
                DetailsVm.RelatedProducts = _context.Houses
                    .Include(x => x.City)
                    .Include(x => x.Category)
                    .Include(x => x.HouseImages)
                    .Include(x => x.UserComments).ThenInclude(x => x.AppUser)
                    .Where(x => x.OwnerId == house.OwnerId && x.Id != house.Id).ToList();

                DetailsVm.TopCategories = _context.Categories
                    .Include(x => x.Houses)
                    .OrderByDescending(x => x.Houses.Count)
                    .Take(5)
                    .ToList();

                DetailsVm.CommentFormVm = new CommentViewModel
                {
                    HouseId = house.Id
                };

                DetailsVm.UserBookingMessageVm = UserMessageVm;
                
                return View("details", DetailsVm);
            }

            UserBookingMessage NewMessage = new UserBookingMessage
            {
                AppUserId = user.Id,
                Message = UserMessageVm.Message,
                Fullname = UserMessageVm.Fullname,
                Email = UserMessageVm.Email,
                CreatedAt = DateTime.UtcNow.AddHours(4),
                UpdatedAt = DateTime.UtcNow.AddHours(4)
            };

            house.UserBookingMessages.Add(NewMessage);
            _context.SaveChanges();

            return RedirectToAction("details", new { id= UserMessageVm.HouseId});
            

        }

        //================================
        // Order a house
        // Only members send order request
        //================================
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> OrderHouse(int id)
        {
            var user =await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NotFound();

            var House = _context.Houses.FirstOrDefault(x=> id== x.Id);
            if (House == null)
                return NotFound();


            return Ok();
        }

        //================================
        // Wishlist 
        // creates new wishlist item removes if this item exists
        //================================
        public async Task<IActionResult> SetItem(int id)
        {

            var house = _context.Houses.Include(x=> x.HouseImages).FirstOrDefault(x => x.Id == id);
            if (house == null)
                return NotFound();


            List<int> WishlistHouseIds = new List<int>();

            AppUser user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = _context.AppUsers.Include(x=>x.WishlistItems).FirstOrDefault(x=> x.UserName == User.Identity.Name);
            }
            if (user != null)
            {

                if(!user.WishlistItems.Any(x=> x.HouseId == house.Id))
                {
                    // create if does not exist
                  WishlistItem wishlistItem = new WishlistItem { HouseId = house.Id };
                  user.WishlistItems.Add(wishlistItem);
                }
                else
                {
                    // delete if exists
                    user.WishlistItems.RemoveAll(x => x.HouseId == house.Id);
                }
                _context.SaveChanges();

                WishlistHouseIds = user.WishlistItems.Select(x => x.HouseId).ToList() ;
            }
            else
            {
                List<int> HouseIds = new List<int>();

                var wishlistStr =  Request.Cookies["wishlist"];

                if (wishlistStr != null)
                    HouseIds = JsonConvert.DeserializeObject < List<int> > (wishlistStr);

                if (!HouseIds.Contains(id))
                {
                   HouseIds.Add(house.Id);
                }
                else
                {
                    HouseIds.Remove(id);
                }

                Response.Cookies.Append("wishlist", JsonConvert.SerializeObject(HouseIds));
                WishlistHouseIds = HouseIds;
            }
            var houses = _context.Houses.Include(x=> x.HouseImages).Where(x => WishlistHouseIds.Contains(x.Id)).ToList();
            return PartialView("_WishlistItemsPartial", houses);
        }
        
        public async Task<int> GetWishlistCount()
        {
            // SetItem action returns only partial view, in js i need to take wislist item count
            //this action returns the count of items in the wishlist

            int count = 0;
            AppUser? user = null;
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                user = _context.AppUsers.Include(x=> x.WishlistItems).FirstOrDefault(x=> x.UserName == User.Identity.Name);
            }

            if(user != null)
            {
                count = user.WishlistItems.Count;
            }
            else
            {
                List<int> HouseIds = new List<int>();

                var wishlistStr = Request.Cookies["wishlist"];

                if (wishlistStr != null)
                    count = JsonConvert.DeserializeObject<List<int>>(wishlistStr).Count;
            }

            return count;
        }


    }
}
