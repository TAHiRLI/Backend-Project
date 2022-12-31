using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NuGet.Packaging.Rules;
using Quarter.Areas.Admin.ViewModels;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using System.Data;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]

    public class UserController : Controller
    {
        private readonly QuarterDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public UserController(QuarterDbContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this._context = context;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }

        public IActionResult Users(int? page)
        {
            var Users = _context.AppUsers
                .Include(x => x.Comments)
                .Where(x => !x.IsAdmin)
                .ToList();




            int pageSize = 5;
            Pagination<AppUser> paginatedList = new Pagination<AppUser>();

            ViewBag.Users = paginatedList.GetPagedNames(Users, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;

            if (ViewBag.Users == null)
                return NotFound();

            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult AdminUsers(int? page)
        {
            var Users = _context.AppUsers
                .Include(x => x.Comments)
                .Where(x => x.IsAdmin)
                .ToList();

            int pageSize = 5;
            Pagination<AppUser> paginatedList = new Pagination<AppUser>();

            ViewBag.Users = paginatedList.GetPagedNames(Users, page, pageSize);
            ViewBag.PageNumber = (page ?? 1);
            ViewBag.PageSize = pageSize;

            if (ViewBag.Users == null)
                return NotFound();

            return View();
        }
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateUser(UserViewModel adminVm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(adminVm);
            }

            if (await _userManager.FindByEmailAsync(adminVm.Email) != null)
            {
                ModelState.AddModelError("Email", "This email is already taken");
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(adminVm);
            }

            if (await _userManager.FindByNameAsync(adminVm.Username) != null)
            {
                ViewBag.Roles = _roleManager.Roles.ToList();
                ModelState.AddModelError("Username", "This Username is already taken");
                return View(adminVm);
            }

            AppUser newUser = new AppUser
            {
                Fullname = adminVm.Username,
                UserName = adminVm.Username,
                Email = adminVm.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(newUser, adminVm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    ViewBag.Roles = _roleManager.Roles.ToList();
                    return View(adminVm);
                }
            }

            foreach (var roleName in adminVm.RoleNames)
            {
                var role = _roleManager.Roles.FirstOrDefault(x => x.Name == roleName);
                if (role != null)
                {
                    await _userManager.AddToRoleAsync(newUser, role.Name);
                    if (role.Name == "SuperAdmin" || role.Name == "Admin")
                    {
                        newUser.IsAdmin = true;
                    }

                }
            }
            _context.SaveChanges();

            return RedirectToAction("AdminUsers");
        }



        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            EditUserViewModel UserVm = new EditUserViewModel();
            IList<string> stringRoles = await _userManager.GetRolesAsync(user);
            foreach (var roleStr in stringRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleStr);
                if (role != null)
                    UserVm.RoleNames.Add(role.Name);
            }

            UserVm.UserId = user.Id;
            ViewBag.Roles = _roleManager.Roles.ToList();

            return View(UserVm);
        }
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EditUser(EditUserViewModel UserVm)
        {
            var user = await _userManager.FindByIdAsync(UserVm.UserId);
            if (user == null)
                return NotFound();
            if (!ModelState.IsValid)
            {
                UserVm.UserId = user.Id;
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View();
            }




            var Roles =await _userManager.GetRolesAsync(user);

            var RemovedRoles = Roles.Where(x => !UserVm.RoleNames.Contains(x)).ToList();
                
            foreach (var role in RemovedRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            var AddedRoles = _roleManager.Roles.Where(x => UserVm.RoleNames.Contains(x.Name)).ToList();
            foreach (var role in AddedRoles)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }
            _context.SaveChanges();
            // check if he is still admin
            bool isAdmin = false;
            foreach (var role in await _userManager.GetRolesAsync(user))
            {
                if(role == "SuperAdmin" || role == "Admin")
                {
                    isAdmin = true;
                    break;
                }
            }
            user.IsAdmin = isAdmin;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("AdminUsers");
        }







        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();
            await _userManager.DeleteAsync(user);

            return RedirectToAction("AdminUsers");

        }

    }
}
