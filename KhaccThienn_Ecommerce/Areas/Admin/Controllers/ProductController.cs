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
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace KhaccThienn_Ecommerce.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _toast;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public ProductController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, INotyfService service)
        {
            _context = context;
            _environment = environment;
            _toast = service;
        }

        // GET: Admin/Product
        [HttpGet]
        public async Task<IActionResult> Index(string? name, string? sort, int page = 1)
        {
            int limit = 3;
            var applicationDbContext = await _context.Products.Include(p => p.Category).OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
            if (!string.IsNullOrEmpty(name))
            {
                Console.WriteLine(name);

                ViewBag.names = name;
                applicationDbContext = await _context.Products.Include(p => p.Category).Where(x => x.Name.Contains(name)).OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;

                Console.WriteLine(sort);
                switch (sort)
                {
                    case "Id-ASC":
                        applicationDbContext = await _context.Products.Include(p => p.Category).OrderBy(x => x.Id).ToPagedListAsync(page, limit);
                        break;
                    case "Id-DESC":
                        applicationDbContext = await _context.Products.Include(p => p.Category).OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
                        break;

                    case "Name-ASC":
                        applicationDbContext = await _context.Products.Include(p => p.Category).OrderBy(x => x.Name).ToPagedListAsync(page, limit);
                        break;
                    case "Name-DESC":
                        applicationDbContext = await _context.Products.Include(p => p.Category).OrderByDescending(x => x.Name).ToPagedListAsync(page, limit);
                        break;

                    case "CreatedDate-ASC":
                        applicationDbContext = await _context.Products.Include(p => p.Category).OrderBy(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;
                    case "CreatedDate-DESC":
                        applicationDbContext = await _context.Products.Include(p => p.Category).OrderByDescending(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;

                    case "UpdatedDate-ASC":
                        applicationDbContext = await _context.Products.Include(p => p.Category).OrderBy(x => x.Updated_Date).ToPagedListAsync(page, limit);
                        break;
                    case "UpdatedDate-DESC":
                        applicationDbContext = await _context.Products.Include(p => p.Category).OrderByDescending(x => x.Updated_Date).ToPagedListAsync(page, limit);
                        break;
                }

            }
            return View( applicationDbContext);
        }

        // GET: Admin/Product/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Product/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories.Where(x => x.Status == 1).ToList(), "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(x => x.Status == 1).ToList(), "Id", "Name");
            return View();
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile fileUpload, [Bind("Name,Image,Price,SalePrice,Status,Descriptions,CategoryId")] Product product)
        {
            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "product", fileUpload.FileName);

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }
                product.Image = fileUpload.FileName;
            }
            else
            {
                ModelState.AddModelError("Image", "The Product's image cannot be null.");
            }

            if (_context.Products.Any(x => x.Name == product.Name))
            {
                ModelState.AddModelError("Name",
                                  "The Product Name is already in use.");
            }

            if (product.SalePrice > product.Price)
            {
                ModelState.AddModelError("SalePrice", "The Product's Sale Price nust be less than Price.");
            }

            if (ModelState.IsValid)
            {
                product.Created_Date = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                _toast.Success("Added Product !", 3);
                return RedirectToAction(nameof(Index));
            }
            _toast.Error("Add Product Failed !", 3);

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Product/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? oldImage, IFormFile? fileUpload, [Bind("Id,Name,Image,Price,SalePrice,Status,Descriptions,CategoryId,Created_Date")] Product product)
        {
            Console.WriteLine($"Old Image: {oldImage}");
            if (id != product.Id)
            {
                return NotFound();
            }
            if (_context.Products.Any(x => x.Name == product.Name && x.Id != product.Id))
            {
                ModelState.AddModelError("Name",
                                  "The Category Name is already in use.");
            }

            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "product", fileUpload.FileName);

                var pathOldFile = Path.Combine(rootPath, "wwwroot", "Uploads", "product", oldImage);

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }

                if (!string.IsNullOrEmpty(oldImage))
                {
                    System.IO.File.Delete(pathOldFile);
                }
                
                
                product.Image = fileUpload.FileName;

            }
            else
            {
                product.Image = oldImage;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.Updated_Date = DateTime.Now;
                    _context.Update(product);
                    _toast.Success("Updated Product !", 3);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            _toast.Error("Update Product Failed !", 3);

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Product/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                var rootPath = _environment.ContentRootPath;
                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "product",product.Image);

                try
                {
                    // Attempt to delete the file
                    System.IO.File.Delete(path);
                    Console.WriteLine("File deleted successfully.");
                    _toast.Success("Deleted Product !", 3);

                    _context.Products.Remove(product);
                }
                catch (IOException e)
                {
                    // Handle any exceptions that may occur during the file deletion
                    Console.WriteLine($"Error deleting file: {e.Message}");
                }

            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
