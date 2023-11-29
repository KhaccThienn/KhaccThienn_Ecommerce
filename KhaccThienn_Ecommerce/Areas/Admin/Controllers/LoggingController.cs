using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Models.DataModels;
using KhaccThienn_Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IMapper _mapper;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public LoggingController(INotyfService toastNotification, ApplicationDbContext context, IMapper mapper, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            _toastNotification = toastNotification;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Details()
        {

            var userId = User.Claims.ToList().Take(1).FirstOrDefault().Value;
            Console.WriteLine($"UserId: {userId}");
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString() == userId);
            var userInfo = _mapper.Map<UpdateAccountViewModels>(user);
            return View(userInfo);

        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int id, string? oldImage, string? oldPassword, IFormFile? fileUpload, [Bind("Id,FirstName,LastName,Username,Email,Phone,Address,Avatar,RoleId,Created_Date")] UpdateAccountViewModels user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (_context.Users.Any(x => x.Email == user.Email && x.Id != user.Id))
            {
                ModelState.AddModelError("Email",
                                  "This Email is currently in use.");
            }

            if (_context.Users.Any(x => x.Phone == user.Phone && x.Id != user.Id))
            {
                ModelState.AddModelError("Phone",
                                  "This Email is currently in use.");
            }

            if (_context.Users.Any(x => x.Username == user.Username && x.Id != user.Id))
            {
                ModelState.AddModelError("Username",
                                  "This Username is currently in use.");
            }

            //if (!string.IsNullOrEmpty(user.Password) && oldPassword != user.Password)
            //{
            //    ModelState.AddModelError("Password",
            //                      "Password and OldPassword Does Not Match");
            //}

            //if (string.IsNullOrEmpty(user.Password))
            //{
            //    user.NewPassword = oldPassword;
            //}

            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "user", fileUpload.FileName);

                if (!string.IsNullOrEmpty(oldImage))
                {
                    var pathOldFile = Path.Combine(rootPath, "wwwroot", "Uploads", "user", oldImage);
                    System.IO.File.Delete(pathOldFile);

                }

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }

                user.Avatar = fileUpload.FileName;

            }
            else
            {
                user.Avatar = oldImage;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Updated_Date = DateTime.Now;
                    var userMapper = _mapper.Map<User>(user);
                    userMapper.Password = oldPassword;
                    _context.Update(userMapper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _toastNotification.Success("Update Profile Success !", 3);

                return RedirectToAction(nameof(Details));
            }
            _toastNotification.Error("Update Profile Failed !", 3);
            return RedirectToAction(nameof(Details));

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int id, string? oldPassword, [Bind("Id,FirstName,LastName,Username,Password,NewPassword,ConfirmPassword,Email,Phone,Address,Avatar,RoleId,Created_Date")] UpdateAccountViewModels user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(user.Password) && oldPassword != user.Password)
            {
                ModelState.AddModelError("Password",
                                  "Password and OldPassword Does Not Match");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                user.NewPassword = oldPassword;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Updated_Date = DateTime.Now;
                    var userMapper = _mapper.Map<User>(user);
                    userMapper.Password = user.NewPassword;
                    _context.Update(userMapper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _toastNotification.Success("Update Password Success !", 3);

                return RedirectToAction(nameof(Details));
            }
            _toastNotification.Error("Update Password Failed !", 3);
            return RedirectToAction(nameof(Details));
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

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
