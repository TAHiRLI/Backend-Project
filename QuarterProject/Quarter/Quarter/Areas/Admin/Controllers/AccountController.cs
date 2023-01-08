using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;
using Quarter.Areas.Admin.ViewModels;
using Quarter.Models;
using System.Data;

namespace Quarter.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }
        //public async Task<IActionResult> CreateSuperAdmin()
        //{
        //    AppUser admin = new AppUser
        //    {
        //        Fullname = "Tahir Tahrli",
        //        UserName = "TahirAdmin",
        //        Email = "admin@ad.com",
        //        EmailConfirmed = true,
        //        IsAdmin = true

        //    };

        //    await _userManager.CreateAsync(admin, "admin123");
        //    await _userManager.AddToRoleAsync(admin, "SuperAdmin");

        //    return Ok();
        //}

        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser admin = new AppUser
        //    {
        //        Fullname = "Tahir Tahrli",
        //        UserName = "SamirAdmin",
        //        Email = "admin@adaa.com",
        //        EmailConfirmed = true,
        //        IsAdmin = true
        //    };

        //    await _userManager.CreateAsync(admin, "admin123");
        //    await _userManager.AddToRoleAsync(admin, "Admin");

        //    return Ok(admin);
        //}

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(AdminLoginViewModel LoginVm)
        {
            if (!ModelState.IsValid)
                return View();


            var user = await _userManager.FindByNameAsync(LoginVm.Username);
            if (user == null || (!await _userManager.IsInRoleAsync(user, "SuperAdmin") && !await _userManager.IsInRoleAsync(user, "Admin") ))
                return View();

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(Index), "dashboard");
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Logout()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NotFound();
            await _signInManager.SignOutAsync();

            return RedirectToAction("login", "account");
        }

    }
}
