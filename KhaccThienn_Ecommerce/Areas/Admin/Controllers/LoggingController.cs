using AspNetCoreHero.ToastNotification.Abstractions;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KhaccThienn_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoggingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _toastNotification;
        public LoggingController(INotyfService toastNotification, ApplicationDbContext context)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public IActionResult Index(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username, Password")] LoginViewModel account, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var accFound = await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(x => x.Username == account.Username && x.Password == account.Password);

                if (accFound != null)
                {
                    if (accFound?.RoleId == 0)
                    {
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim("AccountId", accFound.Id.ToString()),
                            new Claim(ClaimTypes.Name, accFound.FirstName + " " + accFound.LastName),
                            new Claim("avatar", accFound.Avatar ?? "default.png"),
                            new Claim(ClaimTypes.Role, accFound.Role.Name),
                        }, CookieAuthenticationDefaults.AuthenticationScheme);

                        var principal = new ClaimsPrincipal(identity);
                        var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return Redirect("/Admin");
                    }

                    _toastNotification.Error("Login Failed, Permission Denied !", 3);

                    return View("Index", account);
                }
            }
            _toastNotification.Error("Login Failed !", 3);

            return View("Index", account);
        }

        public IActionResult Logout()
        {
            _toastNotification.Success("Log Out Successfully !", 3);
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (login.IsCompletedSuccessfully)
            {
                return RedirectToAction("Index", "Logging");
            }
            _toastNotification.Error("Log Out Failed !", 3);
            return RedirectToAction("Index", "Home");

        }
    }
}
