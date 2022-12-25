using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Quarter.DAL;
using Quarter.Models;
using Quarter.ViewModels;

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
                .FirstOrDefault(x => x.Id == id);

            if (house == null)
                return NotFound();


            DetailsViewModel DetailsVm = new DetailsViewModel();
            DetailsVm.House = house;
            DetailsVm.RelatedProducts = _context.Houses
                .Include(x=> x.City)
                .Include(x=> x.Category)
                .Include(x=> x.HouseImages)
                .Where(x => x.OwnerId == house.OwnerId && x.Id != house.Id).ToList();


            DetailsVm.TopCategories = _context.Categories.Include(x=> x.Houses).OrderByDescending(x=> x.Houses.Count).Take(5).ToList();
            return View(DetailsVm);
        }
    }
}
