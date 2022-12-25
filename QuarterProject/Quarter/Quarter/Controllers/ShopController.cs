﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quarter.DAL;
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
        public IActionResult Index(string? Search =null, List<int>? CategoryIds = null, List<int>? AmenityIds = null, List<int>? CityIds= null, decimal? minPrice= null, decimal? maxPrice = null)
        {
            ViewBag.SelectedCategoryIds = CategoryIds;
            ViewBag.SelectedAmenityIds = AmenityIds;
            ViewBag.SelectedCityIds = CityIds;
            ViewBag.SelectedSearch = Search;
    

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

            if(Search != null)
                house =  house.Where(x=> x.Title.Contains(Search));
            if (CategoryIds != null && CategoryIds.Count >0)
                house = house.Where(x => CategoryIds.Contains(x.CategoryId));
            if (AmenityIds != null && AmenityIds.Count > 0)
                house = house.Where(x => x.HouseAmenities.Any(b=> AmenityIds.Contains(b.AmenityId)));
            if (CityIds != null && CityIds.Count > 0)
                house = house.Where(x => CityIds.Contains(x.CityId));
            if(minPrice != null && maxPrice != null)
                house = house.Where(x => x.Price >= minPrice && x.Price<= maxPrice);


            ShopVm.Houses = house.ToList();

            ShopVm.MaxPrice = _context.Houses.Max(x => x.Price);
            ShopVm.MinPrice = _context.Houses.Min(x => x.Price);
            ViewBag.SelectedMinPrice = minPrice ?? ShopVm.MinPrice;
            ViewBag.SelectedMaxPrice = maxPrice ?? ShopVm.MaxPrice;

            return View(ShopVm);
        }
    }
}