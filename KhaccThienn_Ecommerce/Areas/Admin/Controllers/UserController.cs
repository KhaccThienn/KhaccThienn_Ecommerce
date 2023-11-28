using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Models.DataModels;
using X.PagedList;
using Microsoft.Extensions.Hosting;
using KhaccThienn_Ecommerce.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace KhaccThienn_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private IMapper _mapper;

        public UserController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IMapper mapper)
        {
            _context = context;
            _environment = environment;
            _mapper = mapper;
        }

        // GET: Admin/User
        [HttpGet]
        public async Task<IActionResult> Index(string? name, string? sort, int page = 1)
        {
            int limit = 5;
            var applicationDbContext = await _context.Users.OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
            if (!string.IsNullOrEmpty(name))
            {
                Console.WriteLine(name);

                ViewBag.names = name;
                applicationDbContext = await _context.Users.Where(x => x.LastName.Contains(name)).OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;

                Console.WriteLine(sort);
                switch (sort)
                {
                    case "Id-ASC":
                        applicationDbContext = await _context.Users.OrderBy(x => x.Id).ToPagedListAsync(page, limit);
                        break;
                    case "Id-DESC":
                        applicationDbContext = await _context.Users.OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
                        break;

                    case "FirstName-ASC":
                        applicationDbContext = await _context.Users.OrderBy(x => x.FirstName).ToPagedListAsync(page, limit);
                        break;
                    case "FirstName-DESC":
                        applicationDbContext = await _context.Users.OrderByDescending(x => x.FirstName).ToPagedListAsync(page, limit);
                        break;

                    case "LastName-ASC":
                        applicationDbContext = await _context.Users.OrderBy(x => x.LastName).ToPagedListAsync(page, limit);
                        break;
                    case "LastName-DESC":
                        applicationDbContext = await _context.Users.OrderByDescending(x => x.LastName).ToPagedListAsync(page, limit);
                        break;

                    case "CreatedDate-ASC":
                        applicationDbContext = await _context.Users.OrderBy(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;
                    case "CreatedDate-DESC":
                        applicationDbContext = await _context.Users.OrderByDescending(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;

                    case "UpdatedDate-ASC":
                        applicationDbContext = await _context.Users.OrderBy(x => x.Updated_Date).ToPagedListAsync(page, limit);
                        break;
                    case "UpdatedDate-DESC":
                        applicationDbContext = await _context.Users.OrderByDescending(x => x.Updated_Date).ToPagedListAsync(page, limit);
                        break;
                }

            }
            return View(applicationDbContext);
        }

        // GET: Admin/User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(r => r.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/User/Create
        public IActionResult Create()
        {
            ViewBag.RoleId = new SelectList(_context.Roles, "Id", "Name");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: Admin/User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile fileUpload, User user)
        {
            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "user", fileUpload.FileName);

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }
                user.Avatar = fileUpload.FileName;
            }
            else
            {
                ModelState.AddModelError("Avatar", "The Product's image cannot be null.");
            }

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
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userMapper = _mapper.Map<UpdateAccountViewModels>(user);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", userMapper.RoleId);
            ViewBag.RoleId = new SelectList(_context.Roles, "Id", "Name", userMapper.RoleId);
            return View(userMapper);
        }

        // POST: Admin/User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? oldPassword, string? oldImage, IFormFile? fileUpload, [Bind("Id,FirstName,LastName,Username,Password,NewPassword,ConfirmPassword,Email,Phone,Address,Avatar,RoleId,Created_Date")] UpdateAccountViewModels user)
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

            if (!string.IsNullOrEmpty(user.Password) && oldPassword != user.Password)
            {
                ModelState.AddModelError("Password",
                                  "Password and OldPassword Does Not Match");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                user.NewPassword = oldPassword;
            }

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
                return RedirectToAction(nameof(Index));
            }
            ViewBag.RoleId = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: Admin/User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
