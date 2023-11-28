using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Models.DataModels;
using KhaccThienn_Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Security.Claims;

namespace KhaccThienn_Ecommerce.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly INotyfService _toastNotification;
        private readonly IMapper _mapper;
        public UserController(INotyfService toastNotification, ApplicationDbContext context, IMapper mapper, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            _toastNotification = toastNotification;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task<IActionResult> Details()
        {
            if (HttpContext.Session.GetInt32("userID") != null)
            {
                var userId = HttpContext.Session.GetInt32("userID");
                Console.WriteLine($"UserId: {userId}");
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                var userInfo = _mapper.Map<UpdateAccountViewModels>(user);
                return View(userInfo);
            }
            else
            {
                //return RedirectToAction("Login", "User");
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }
        }

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

        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([Bind("Username, Password")] LoginViewModel account, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var accFound = await _context.Users.FirstOrDefaultAsync(x => x.Username == account.Username && x.Password == account.Password);
                if (accFound != null)
                {
                    //    var identity = new ClaimsIdentity(new[]
                    //    {
                    //    new Claim("AccountId", accFound.Id.ToString()),
                    //    new Claim(ClaimTypes.Name, accFound.Username),
                    //    new Claim(ClaimTypes.Role, accFound.Role.Name),
                    //    new Claim(ClaimTypes.MobilePhone, "09786343")
                    //}, "KhaccThiennSchema");
                    //    var principal = new ClaimsPrincipal(identity);
                    //    var login = HttpContext.SignInAsync("KhaccThiennSchema", principal);

                    HttpContext.Session.SetInt32("userID", accFound.Id);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        _toastNotification.Success("Login Successfully !", 3);
                        return Redirect(returnUrl);
                    }
                    _toastNotification.Success("Login Successfully !", 3);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    /*
                     custom toastnotify: 
                    _toastNotification.Custom(string? message, int? duration, string? color, string? icon)
                     */
                    //_toastNotification.Custom("Please check the settings for your profile - closes in 6 seconds.", 6, "#0c343d", "fa fa-user");
                    _toastNotification.Error("Login Failed !", 3);
                    return View(account);
                }
            }
            else
            {
                _toastNotification.Error("Login Failed, Invalid Account !", 3);
                return View(account);
            }

        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([Bind("FirstName, LastName, Username, Password, Email, Phone")] RegisterViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(x => x.Email == user.Email))
                {
                    ModelState.AddModelError("Email",
                                      "This Email is currently in use.");
                }

                if (_context.Users.Any(x => x.Username == user.Username))
                {
                    ModelState.AddModelError("Username",
                                      "This Username is currently in use.");
                }

                if (_context.Users.Any(x => x.Phone == user.Phone))
                {
                    ModelState.AddModelError("Phone",
                                      "This Email is currently in use.");
                }

                if (ModelState.IsValid)
                {
                    
                    user.Created_Date = DateTime.Now;

                    var userMapper = _mapper.Map<User>(user);
                    userMapper.RoleId = 1;
                    _context.Add(userMapper);
                    await _context.SaveChangesAsync();
                    _toastNotification.Success("Create Account Success !", 3);
                    return RedirectToAction(nameof(Login));
                }
                _toastNotification.Error("Create Account Failed !", 3);

                return View(user);
            }
            else
            {
                /*
                 custom toastnotify: 
                _toastNotification.Custom(string? message, int? duration, string? color, string? icon)
                 */
                //_toastNotification.Custom("Please check the settings for your profile - closes in 6 seconds.", 6, "#0c343d", "fa fa-user");
                _toastNotification.Error("Register Failed !", 3);
                return View(user);
            }
        }

        public IActionResult Logout()
        {
            _toastNotification.Success("Log Out Successfully !", 3);
            HttpContext.Session.Remove("userID");
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
