using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
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
            return View(DetailsVm);
        }
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

            //^ signalr adminpanel message

            return RedirectToAction("details", new { id = house.Id });

        }

    



    }
}
