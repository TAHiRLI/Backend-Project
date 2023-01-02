using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using MimeKit.Text;
using MimeKit;
using Quarter.DAL;
using Quarter.Helpers;
using Quarter.Models;
using Quarter.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NuGet.Protocol;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Asn1.X509;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Quarter.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly QuarterDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment _env;

        public string MyEmail = "deangelo81@ethereal.email";
        public string MyPassword = "HNANVb3Zsh6c5AndXG";

        public AccountController(UserManager<AppUser> userManager, QuarterDbContext context, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, IWebHostEnvironment env)
        {

            this._userManager = userManager;
            this._context = context;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
            this._env = env;
        }

        // create roles 
        // public async Task<IActionResult> CreateRoles()
        // {
        //IdentityRole role1 = new IdentityRole("SuperAdmin");
        //IdentityRole role2 = new IdentityRole("Admin");
        //IdentityRole role3 = new IdentityRole("Member");
        // IdentityRole role4 = new IdentityRole("Visitor");


        //await _roleManager.CreateAsync(role1);
        //await _roleManager.CreateAsync(role2);
        //await _roleManager.CreateAsync(role3);
        // await _roleManager.CreateAsync(role4);

        //   return Ok();
        //  }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemmberRegisterViewModel RegisterVm)
        {


            if (!ModelState.IsValid)
                return View();



            if (await _userManager.FindByEmailAsync(RegisterVm.Email) != null)
            {
                ModelState.AddModelError("Email", "This email is already taken");
                return View();
            }

            if (await _userManager.FindByNameAsync(RegisterVm.Username) != null)
            {
                ModelState.AddModelError("Username", "This Username is already taken");
                return View();
            }


            AppUser user = new AppUser
            {
                Fullname = RegisterVm.Fullname,
                Email = RegisterVm.Email,
                UserName = RegisterVm.Username,
                IsSubscribed = RegisterVm.IsSubscribed
            };


            if (RegisterVm.File != null)
            {
                user.UserPhoto = FileManager.Save(RegisterVm.File, _env.WebRootPath, "Uploads/Users", 100);
            }

            var result = await _userManager.CreateAsync(user, RegisterVm.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);

                }
                return View();
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action(nameof(ConfirmEmail), "account", new { token = token, email = user.Email }, Request.Scheme); ;

            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(MyEmail));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Please, verify your mail address";
            email.Body = new TextPart(TextFormat.Html) { Text = $"Click <a href=\"{url}\" >here</a> to verify your email" };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(MyEmail, MyPassword);
            smtp.Send(email);
            smtp.Disconnect(true);


            //^ signalr toastr



            await _userManager.AddToRoleAsync(user, "Visitor");

            return RedirectToAction("login");
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound();

            

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return NotFound();

            await _userManager.RemoveFromRoleAsync(user, "Visitor");
            await _userManager.AddToRoleAsync(user, "Member");


            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction(nameof(Login));

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

            if (result.Succeeded)
                return RedirectToAction(nameof(Index), "home");

            else
            {
                AppUser user = new AppUser
                {
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    EmailConfirmed = true,
                    
                };
                IdentityResult identityResult = await _userManager.CreateAsync(user);
                if (identityResult.Succeeded)
                {
                    identityResult = await _userManager.AddLoginAsync(user, info);
                    if (identityResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Member");
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction(nameof(Index), "home");
                    }
                }
            }


           

            return NotFound();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(MemberLoginViewModel LoginVm, string? ReturnUrl = null)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByNameAsync(LoginVm.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "Username or password is inccorrect");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, LoginVm.Password, true, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Too many attempts, please try again after 5 minutes");
                return View();
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action(nameof(ConfirmEmail), "account", new { token = token, email = user.Email }, Request.Scheme); ;

                // create email message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(MyEmail));
                email.To.Add(MailboxAddress.Parse(user.Email));
                email.Subject = "Please, verify your mail address";
                email.Body = new TextPart(TextFormat.Html) { Text = $"Click <a href=\"{url}\" >here</a> to verify your email" };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(MyEmail, MyPassword);
                smtp.Send(email);
                smtp.Disconnect(true);

                ModelState.AddModelError("", "Please verify your email");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username of password is incorredt");
                return View();
            }

            if (ReturnUrl != null)
                return Redirect(ReturnUrl);

            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel ForgotVm)
        {
            var user = await _userManager.FindByEmailAsync(ForgotVm.Email);
            if (user == null)
                return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("VerifyPasswordReset", "account", new { email = user.Email, token = token }, Request.Scheme);


            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(MyEmail));
            email.To.Add(MailboxAddress.Parse(ForgotVm.Email));
            email.Subject = "Please, verify your mail address";
            email.Body = new TextPart(TextFormat.Html) { Text = $"Click <a href=\"{url}\" >here</a> to verify your email" };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(MyEmail, MyPassword);
            smtp.Send(email);
            smtp.Disconnect(true);


            //^ signalr toastr

            return View();
        }
        public async Task<IActionResult> VerifyPasswordReset(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token))
                return NotFound();

            TempData["token"] = token;
            TempData["email"] = email;
            return RedirectToAction("resetPassword");
        }
        public IActionResult ResetPassword()
        {
            var email = TempData["email"];
            var token = TempData["token"];


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel ResetVm)
        {
            if (ResetVm.Email == null || ResetVm.Token == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["email"] = ResetVm.Email;
                TempData["token"] = ResetVm.Token;
                return View();
            }
            var user = await _userManager.FindByEmailAsync(ResetVm.Email);

            if (user == null)
                return NotFound();

            var result = await _userManager.ResetPasswordAsync(user, ResetVm.Token, ResetVm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["email"] = ResetVm.Email;
                TempData["token"] = ResetVm.Token;
                return View();
            }

            return RedirectToAction("login");
        }
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Logout()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NotFound();
            await _signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NotFound();
            ProfileViewModel ProfileVm = new ProfileViewModel();
            ProfileVm.Orders = _context.Orders.Include(x => x.House).Where(x => x.AppUserId == user.Id).OrderByDescending(x=> x.CreatedAt).Take(15).ToList();
            ProfileVm.ProfileEditVm.Fullname = user.Fullname;
            ProfileVm.ProfileEditVm.Email = user.Email;
            ProfileVm.ProfileEditVm.Username = user.UserName;
            ProfileVm.ProfileEditVm.UserPhoto = user.UserPhoto;
            ProfileVm.UserBookingMessages = _context.UserBookingMessages
                .OrderByDescending(x => x.CreatedAt)
                .Include(x => x.BookingRequestReply)
                .Include(x => x.House)
                .Where(x => x.AppUserId == user.Id && x.IsReplied)
                .Take(10)
                .ToList();

            return View(ProfileVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> Profile(ProfileEditViewModel MemberVm)
        {
            ProfileViewModel ProfileVm = new ProfileViewModel();
            ProfileVm.ProfileEditVm = MemberVm;

            if (!ModelState.IsValid)
                return View(ProfileVm);


            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NotFound();

            if (MemberVm.Email.ToUpper() != user.NormalizedEmail && _context.AppUsers.Any(x => x.NormalizedEmail == MemberVm.Email.ToUpper()))
                ModelState.AddModelError("Email", "This email allready taken");
            if (MemberVm.Username.ToUpper() != user.NormalizedUserName && _context.AppUsers.Any(x => x.NormalizedUserName == MemberVm.Username.ToUpper()))
                ModelState.AddModelError("Email", "This email allready taken");

            if (!ModelState.IsValid)
                return View(ProfileVm);

            IdentityResult isPasswordUpdated = IdentityResult.Success;
            IdentityResult isUserUpdated = IdentityResult.Success;

            if (MemberVm.CurrentPassword != null || MemberVm.NewPassword != null)
            {
                if(MemberVm.CurrentPassword == null || !await _userManager.CheckPasswordAsync(user, MemberVm.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Password is incorrect");
                    return View(ProfileVm);
                }
                 isPasswordUpdated = await _userManager.ChangePasswordAsync(user, MemberVm.CurrentPassword, MemberVm.NewPassword);
                if (!isPasswordUpdated.Succeeded)
                {
                    foreach (var error in isPasswordUpdated.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                        return View(ProfileVm);
                }
            }
            if(MemberVm.File != null)
            {
                if(user.UserPhoto != "defaultUser.jpeg")
                FileManager.Delete(_env.WebRootPath, "Uploads/Users", user.UserPhoto);

                user.UserPhoto = FileManager.Save(MemberVm.File, _env.WebRootPath, "Uploads/Users", 100);
            }

            user.UserName = MemberVm.Username;
            user.Fullname = MemberVm.Fullname;

            if (MemberVm.Email.ToUpper() != user.NormalizedEmail)
            {
                user.Email = MemberVm.Email;
                user.EmailConfirmed = false;

            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action(nameof(ConfirmEmail), "account", new { token = token, email = MemberVm.Email }, Request.Scheme); ;

            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(MyEmail));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Please, verify your mail address";
            email.Body = new TextPart(TextFormat.Html) { Text = $"Click <a href=\"{url}\" >here</a> to verify your email" };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(MyEmail, MyPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
            //^signalr toastr



            isUserUpdated = await _userManager.UpdateAsync(user);


            using( var transaction = _context.Database.BeginTransaction())
            {
                if(isUserUpdated.Succeeded && isPasswordUpdated.Succeeded)
                {
                    _context.SaveChanges();
                    transaction.Commit();
                }
                else
                {
                    ModelState.AddModelError("", "something went wrong");
                    return View(ProfileVm);
                }

            }
            await _signInManager.SignOutAsync();

            return RedirectToAction("login", "account");
        }
    }
}
