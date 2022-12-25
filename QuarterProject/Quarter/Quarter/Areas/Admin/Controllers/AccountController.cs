using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;
using Quarter.Areas.Admin.ViewModels;
using Quarter.Models;

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
        //public async Task<IActionResult>  CreateAdmin()
        // {
        //     AppUser admin = new AppUser
        //     {
        //         Fullname = "Tahir Tahrli",
        //         UserName = "TahirAdmin",
        //         Email = "admin@ad.com",
        //         EmailConfirmed = true
        //     };

        //     await _userManager.CreateAsync(admin, "admin123");
        //     await _userManager.AddToRoleAsync(admin,"SuperAdmin");

        //     return Ok();
        // }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
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
    }
}
