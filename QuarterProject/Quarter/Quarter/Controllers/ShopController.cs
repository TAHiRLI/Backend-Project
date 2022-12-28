using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using Quarter.ViewModels;

namespace Quarter.Controllers
{
    public class ShopController : Controller
    {
        private readonly QuarterDbContext _context;

        public ShopController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index(
            int? page,
            string? search, 
            List<int>? categoryIds,
            List<int>? amenityIds,
            List<int>? cityIds,
            decimal?minPrice,
            decimal? maxPrice,
            int pageSize = 4,
            string? sort = "AZ"
            )
        {
            ViewBag.SelectedCategoryIds = categoryIds;
            ViewBag.SelectedAmenityIds = amenityIds;
            ViewBag.SelectedCityIds = cityIds;
            ViewBag.SelectedSearch = search;
            ViewBag.SelectedSort = sort;
            ViewBag.SelectedPageSize = pageSize;    
    

            ShopViewModel ShopVm = new ShopViewModel
            {   
            Cities = _context.Cities.Include(x=> x.Houses).ToList(),
            Amenities= _context.Amenities.Include(x=>x.HouseAmenities).ToList(),
            Categories = _context.Categories.Include(x=> x.Houses).ToList()

            };

            var house = _context.Houses
                .Include(x=> x.HouseImages)
                .Include(x=> x.City)
                .Include(x=> x.Owner)
                .Include(x=> x.Category)
                .Include(x=> x.HouseAmenities)
                .AsQueryable();

            if(search != null)
                house =  house.Where(x=> x.Title.Contains(search));
            if (categoryIds != null && categoryIds.Count >0)
                house = house.Where(x => categoryIds.Contains(x.CategoryId));
            if (amenityIds != null && amenityIds.Count > 0)
                house = house.Where(x => x.HouseAmenities.Any(b=> amenityIds.Contains(b.AmenityId)));
            if (cityIds != null && cityIds.Count > 0)
                house = house.Where(x => cityIds.Contains(x.CityId));
            if(minPrice != null && maxPrice != null)
                house = house.Where(x => x.Price >= minPrice && x.Price<= maxPrice);

            switch (sort)
            {
                case "ZA":
                    house = house.OrderByDescending(x => x.Title);
                    break;
                case "HighToLow":
                    house = house.OrderByDescending(x => x.Price);
                    break;
                case "LowToHigh":
                    house = house.OrderBy(x => x.Price);
                    break;

                default:
                    house = house.OrderBy(x => x.Title);
                    break;
            }


            ShopVm.Houses = house.ToList();

            ShopVm.MaxPrice = _context.Houses.Max(x => x.Price);
            ShopVm.MinPrice = _context.Houses.Min(x => x.Price);
            ViewBag.SelectedMinPrice = minPrice ?? ShopVm.MinPrice;
            ViewBag.SelectedMaxPrice = maxPrice ?? ShopVm.MaxPrice;



          
            Pagination<House> paginatedList = new Pagination<House>();

            ViewBag.Houses = paginatedList.GetPagedNames(house.ToList(), page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Houses == null)
                return NotFound();



            return View(ShopVm);
        }
    }
}
