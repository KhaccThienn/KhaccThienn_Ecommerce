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
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace KhaccThienn_Ecommerce.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly INotyfService _toastNotification;
        public CategoryController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, INotyfService notyfService)
        {
            _context = context;
            _environment = environment;
            _toastNotification = notyfService;
        }

        // GET: Admin/Category
        [HttpGet]
        public async Task<IActionResult> Index(string? name, string? sort, int? page = 1)
        {
            int limit = 3;
            page = page <= 1 ? 1 : page;
            ViewBag.names = name;
            ViewBag.sorts = sort;

            var categories = _context.Categories.OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
            if (!string.IsNullOrEmpty(name))
            {
                Console.WriteLine(name);

                categories = _context.Categories.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
            }
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;

                Console.WriteLine(sort);
                switch (sort)
                {
                    case "Id-ASC":
                        categories = _context.Categories.OrderBy(x => x.Id).ToPagedListAsync(page, limit);
                        break;
                    case "Id-DESC":
                        categories = _context.Categories.OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
                        break;

                    case "Name-ASC":
                        categories = _context.Categories.OrderBy(x => x.Name).ToPagedListAsync(page, limit);
                        break;
                    case "Name-DESC":
                        categories = _context.Categories.OrderByDescending(x => x.Name).ToPagedListAsync(page, limit);
                        break;

                    case "CreatedDate-ASC":
                        categories = _context.Categories.OrderBy(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;
                    case "CreatedDate-DESC":
                        categories = _context.Categories.OrderByDescending(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;

                    case "UpdatedDate-ASC":
                        categories = _context.Categories.OrderBy(x => x.Updated_Date).ToPagedListAsync(page, limit);
                        break;
                    case "UpdatedDate-DESC":
                        categories = _context.Categories.OrderByDescending(x => x.Updated_Date).ToPagedListAsync(page, limit);
                        break;
                }

            }
            return View(await categories);
        }

        // GET: Admin/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile fileUpload, [Bind("Name,Image,Status")] Category category)
        {
            if (_context.Categories.Any(x => x.Name == category.Name))
            {
                ModelState.AddModelError("Name",
                                  "The Category Name is already in use.");
            }
            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "category", fileUpload.FileName);

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }
                category.Image = fileUpload.FileName;
            }
            else
            {
                ModelState.AddModelError("Image", "The Category's Display Image cannot be null.");
            }


            if (ModelState.IsValid)
            {
                category.Created_Date = DateTime.Now;

                _context.Add(category);
                _toastNotification.Success("Added New Category !", 3);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            _toastNotification.Error("Having An Error When Add New Category !", 3);

            return View(category);
        }

        // GET: Admin/Category/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? oldImage, IFormFile? fileUpload, [Bind("Id,Name,Status,Created_Date")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (_context.Categories.Any(x => x.Name == category.Name && x.Id != category.Id))
            {
                ModelState.AddModelError("Name",
                                  "The Category Name is already in use.");
            }

            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "category", fileUpload.FileName);

                if (!string.IsNullOrEmpty(oldImage))
                {
                    var pathOldFile = Path.Combine(rootPath, "wwwroot", "Uploads", "category", oldImage);
                    System.IO.File.Delete(pathOldFile);

                }

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }

                category.Image = fileUpload.FileName;

            }
            else
            {
                category.Image = oldImage;
            }


            if (ModelState.IsValid)
            {
                try
                {
                    category.Updated_Date = DateTime.Now;
                    _context.Update(category);
                    _toastNotification.Success("Category Updated !", 3);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            _toastNotification.Error("Category Failed !", 3);
            return View(category);
        }

        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                var rootPath = _environment.ContentRootPath;
                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "category", category?.Image);
                
                if (category?.Products?.Count > 0)
                {
                    _toastNotification.Error("Delete Category Failed !", 3);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    try
                    {
                        // Attempt to delete the file
                        System.IO.File.Delete(path);
                        Console.WriteLine("File deleted successfully.");
                        _context.Categories.Remove(category);
                        _toastNotification.Success("Delete Category successfully !", 3);

                    }
                    catch (IOException e)
                    {
                        // Handle any exceptions that may occur during the file deletion
                        Console.WriteLine($"Error deleting file: {e.Message}");
                    }

                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
