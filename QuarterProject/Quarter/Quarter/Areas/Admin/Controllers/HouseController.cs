using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NuGet.Protocol;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using System.Data;
using System.Security.AccessControl;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class HouseController : BaseController
        {
        private readonly QuarterDbContext _context;
        private readonly IWebHostEnvironment _env;

        public HouseController(QuarterDbContext context, IWebHostEnvironment env)
        {
            this._context = context;
            this._env = env;
        }
        public IActionResult Index(int? page)
        {
            var Houses = _context.Houses
                .Include(x => x.Category)
                .Include(x => x.Owner)
                .Include(x => x.City)
                .Include(x => x.HouseImages)
                .Include(x=> x.UserComments)
                .OrderByDescending(x=> x.UserComments.Max(x=> x.CreatedAt))
                .ToList();



            int pageSize = 5;
            Pagination<House> paginatedList = new Pagination<House>();

            ViewBag.Houses = paginatedList.GetPagedNames(Houses, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Houses == null)
                return NotFound();




            return View();
        }
        public IActionResult Create()
        {
            ViewBag.Cities = _context.Cities.ToList();
            ViewBag.Amenities = _context.Amenities.ToList();
            ViewBag.Owners = _context.Owners.ToList();
            ViewBag.Categories = _context.Categories.ToList();

            return View();
        }
        [HttpPost]
        public IActionResult Create(House house)
        {
            if (!_context.Cities.Any(x => x.Id == house.CityId))
                ModelState.AddModelError("CityId", "City not found");
            if (!_context.Categories.Any(x => x.Id == house.CategoryId))
                ModelState.AddModelError("CategoryId", "Category not found");
            if (!_context.Owners.Any(x => x.Id == house.OwnerId))
                ModelState.AddModelError("OwnerId", "Owner not found");
            if (house.File == null)
                ModelState.AddModelError("File", "Poster Image is required");
            if (house.AmenityIds != null)
            {
                foreach (var amenityId in house.AmenityIds)
                {
                    if (!_context.Amenities.Any(x => x.Id == amenityId))
                    {
                        ModelState.AddModelError("AmenityIds", "Amenity not found");
                        break;
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = _context.Cities.ToList();
                ViewBag.Amenities = _context.Amenities.ToList();
                ViewBag.Owners = _context.Owners.ToList();
                ViewBag.Categories = _context.Categories.ToList();
                return View();
            }

            HouseImage posterImage = new HouseImage
            {
                ImageUrl = FileManager.Save(house.File, _env.WebRootPath, "Uploads/Houses", 100),
                PosterStatus = true,
            };
            house.HouseImages.Add(posterImage);

            if (house.Files != null)
            {
                foreach (var file in house.Files)
                {
                    HouseImage OtherImage = new HouseImage
                    {
                        ImageUrl = FileManager.Save(file, _env.WebRootPath, "Uploads/Houses", 100),
                        PosterStatus = false,
                    };
                    house.HouseImages.Add(OtherImage);
                }

            }

            foreach (var Id in house.AmenityIds)
            {
                HouseAmenity houseAmenity = new HouseAmenity
                {
                    AmenityId = Id
                };
                house.HouseAmenities.Add(houseAmenity);
            }

            house.CreatedAt = DateTime.UtcNow.AddHours(4);
            house.UpdatedAt = DateTime.UtcNow.AddHours(4);

            _context.Houses.Add(house);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit (int id)
        {
            var model = _context.Houses
                .Include(x=> x.HouseAmenities)
                .Include(x=> x.City)
                .Include(x=> x.Category)
                .Include(x=> x.Owner)
                .Include(x=> x.HouseImages)
                .FirstOrDefault(h => h.Id == id);
            if(model == null)
                return NotFound();


            ViewBag.Cities = _context.Cities.ToList();
            ViewBag.Amenities = _context.Amenities.ToList();
            ViewBag.Owners = _context.Owners.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            model.AmenityIds = model.HouseAmenities.Select(x => x.AmenityId).ToList();
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(House house)
        {
            var existHouse = _context.Houses.Include(x=> x.HouseImages).Include(x=> x.HouseAmenities).FirstOrDefault(x => x.Id == house.Id);
            if (existHouse == null)
                return NotFound();

            if (!_context.Cities.Any(x => x.Id == house.CityId))
                ModelState.AddModelError("CityId", "City not found");
            if (!_context.Categories.Any(x => x.Id == house.CategoryId))
                ModelState.AddModelError("CategoryId", "Category not found");
            if (!_context.Owners.Any(x => x.Id == house.OwnerId))
                ModelState.AddModelError("OwnerId", "Owner not found");
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = _context.Cities.ToList();
                ViewBag.Amenities = _context.Amenities.ToList();
                ViewBag.Owners = _context.Owners.ToList();
                ViewBag.Categories = _context.Categories.ToList();
                return View();
            }
           


            existHouse.HouseAmenities.RemoveAll(x => !house.AmenityIds.Contains(x.AmenityId));

            foreach (var amenityId in house.AmenityIds.Where(x=> !existHouse.HouseAmenities.Any(a=> a.AmenityId == x) ).ToList())
            {
                HouseAmenity newHouseAmenity = new HouseAmenity
                {
                    AmenityId = amenityId,
                };
                existHouse.HouseAmenities.Add(newHouseAmenity);
            }


            if(house.File != null)
            {
                var posterImg = existHouse.HouseImages.FirstOrDefault(x => x.PosterStatus == true)?.ImageUrl;
                if (posterImg == null)
                    return NotFound();
                FileManager.Delete(_env.WebRootPath, "Uploads/Houses", posterImg);
                existHouse.HouseImages.FirstOrDefault(x=> x.PosterStatus).ImageUrl = FileManager.Save(house.File, _env.WebRootPath, "Uploads/Houses", 100);
            }


            var removedImages = existHouse.HouseImages.Where(x => !house.HouseImgIds.Contains(x.Id) && x.PosterStatus == false).ToList();

            foreach (var image in removedImages)
            {
                FileManager.Delete(_env.WebRootPath, "Uploads/Books", image.ImageUrl);
                existHouse.HouseImages.Remove(image);
            }

            if (house.Files != null)
            {
                foreach (var file in house.Files)
                {
                    HouseImage OtherImage = new HouseImage
                    {
                        ImageUrl = FileManager.Save(file, _env.WebRootPath, "Uploads/Houses", 100),
                        PosterStatus = false,
                    };
                    existHouse.HouseImages.Add(OtherImage);
                }

            }




            existHouse.Title = house.Title;
            existHouse.Desc = house.Desc;
            existHouse.Location = house.Location;
            existHouse.CategoryId = house.CategoryId;
            existHouse.OwnerId = house.OwnerId;
            existHouse.CityId = house.CityId;
            existHouse.Area = house.Area;
            existHouse.RoomCount = house.RoomCount;
            existHouse.BathroomCount = house.BathroomCount;
            existHouse.ParkingCount = house.ParkingCount; ;
            existHouse.BedroomCount = house.BedroomCount;
            existHouse.DiscountPercent = house.DiscountPercent;
            existHouse.Price = house.Price;
            existHouse.PosterDesc = house.PosterDesc;
            existHouse.IsFeatured = house.IsFeatured;
            existHouse.IsSold = house.IsSold;
            existHouse.UpdatedAt = DateTime.UtcNow.AddHours(4);

            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            var house = _context.Houses.Include(x=> x.HouseImages).FirstOrDefault(x => x.Id == id);
            if (house == null)
                return NotFound();
            foreach (var hImage in house.HouseImages)
            {
                FileManager.Delete(_env.WebRootPath, "Uploads/Houses", hImage.ImageUrl);
            }

            _context.Houses.Remove(house);
            _context.SaveChanges();
            
            return RedirectToAction("index");
        }
        public IActionResult BookingRequest(int? page)
        {
            //shows all the booking requests to the admin

            var Requests = _context.UserBookingMessages
                .Include(x => x.AppUser)
                .Include(x => x.House)
                .OrderByDescending(x=> x.CreatedAt)
                .ToList();


            int pageSize = 5;
            Pagination<UserBookingMessage> paginatedList = new Pagination<UserBookingMessage>();

            ViewBag.Requests = paginatedList.GetPagedNames(Requests, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Requests == null)
                return NotFound();


            return View();
        }
        public IActionResult ReplyToBookingRequest(int id, string replyMessage)
        {
            // js input takes string and replies to request

            var request = _context.UserBookingMessages.FirstOrDefault(x => x.Id == id);
            if (request == null || request.IsReplied)
                return NotFound();

            if (replyMessage.Length > 150) 
                return NotFound();

            BookingRequestReply reply = new BookingRequestReply
            {
                CreatedAt = DateTime.UtcNow.AddHours(4),
                ReplyMessage = replyMessage,
            };

            request.BookingRequestReply = reply;
            request.IsReplied = true;

            _context.SaveChanges();




            

            return Ok();
        }
        public IActionResult GetReplyMessage(int id)
        {
            // gets a request id and returns its reply 

            var replyMessage = _context.UserBookingMessages.Include(x=> x.BookingRequestReply).FirstOrDefault(x => x.Id == id);
            if (replyMessage == null || replyMessage.BookingRequestReply == null)
                return NotFound();

            return Ok(replyMessage.BookingRequestReply);
        }


        //=================================
        // Export as Excel
        //=================================
        [HttpPost]
         
        public IActionResult ExportAsExcell()
        {
            var houses = _context.Houses.ToList();
           using(XLWorkbook wb =new XLWorkbook())
            {
                wb.Worksheets.Add(Common.ToDataTable<House>(houses.ToList()));
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{DateTime.UtcNow.AddHours(4).Date}-houses.xlsx");
                }
            }  
            
            return View();
        }
    }
}
