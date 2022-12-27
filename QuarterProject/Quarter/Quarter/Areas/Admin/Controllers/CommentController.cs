using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using System.Data;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]

    public class CommentController : Controller
    {
        private readonly QuarterDbContext _context;

        public CommentController(QuarterDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index(int id, int? page)
        {

           var house = _context.Houses.Include(x=> x.UserComments).ThenInclude(x=> x.AppUser).FirstOrDefault(x => x.Id == id);
           if(house == null)
                return NotFound();
            
            
            int pageSize = 5;
            var Comments = house.UserComments.OrderByDescending(x=> x.CreatedAt).ToList();
            Pagination<UserComment> paginatedList = new Pagination<UserComment>();

            ViewBag.Comments = paginatedList.GetPagedNames(Comments, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;
            if (ViewBag.Comments == null)
                return NotFound();
            return View();
        }
        public IActionResult Approve(int id)
        {
            var comment = _context.UserComments.FirstOrDefault(x => x.Id == id);
            if (comment == null)
                return NotFound();

            comment.IsApproved = true;
            _context.SaveChanges();

            return RedirectToAction("index",  new {id = comment.HouseId});
        }
        public IActionResult Deny(int id)
        {
            var comment = _context.UserComments.FirstOrDefault(x => x.Id == id);
            if (comment == null)
                return NotFound();

            _context.UserComments.Remove(comment);
            _context.SaveChanges();

            return RedirectToAction("index",new { id = comment.HouseId });
        }
    }
}
