﻿using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Office2010.Excel;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using X.PagedList;

namespace KhaccThienn_Ecommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _toastNotification;
        public ProductController(ApplicationDbContext context, INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? name, string? sort, int? cateId, int page = 1, int limit = 9)
        {
            page = page <= 1 ? 1 : page;
            var products = await _context.Products.ToPagedListAsync(page, limit);

            if (cateId != null)
            {
                products = await _context.Products.Where(x => x.Category.Id == cateId).ToPagedListAsync(page, limit);
            }

            var userId = HttpContext.Session.GetInt32("userID");
            var userFound = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            ViewBag.userFound = null;
            if (userFound != null)
            {
                ViewBag.userFound = userFound;
                Console.WriteLine($"UserID Header: {userFound?.Id}");
            }

            ViewBag.categories = null;
            var categories = await _context.Categories.Include(c => c.Products).ToListAsync();
            if (categories != null)
            {
                ViewBag.categories = categories;
            }

            if (!string.IsNullOrEmpty(name))
            {
                Console.WriteLine(name);

                ViewBag.names = name;
                products = await _context.Products.Include(p => p.Category).Where(x => x.Name.Contains(name)).OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;

                Console.WriteLine(sort);
                switch (sort)
                {
                    case "Id-ASC":
                        products = await _context.Products.Include(p => p.Category).OrderBy(x => x.Id).ToPagedListAsync(page, limit);
                        break;
                    case "Id-DESC":
                        products = await _context.Products.Include(p => p.Category).OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
                        break;

                    case "Name-ASC":
                        products = await _context.Products.Include(p => p.Category).OrderBy(x => x.Name).ToPagedListAsync(page, limit);
                        break;
                    case "Name-DESC":
                        products = await _context.Products.Include(p => p.Category).OrderByDescending(x => x.Name).ToPagedListAsync(page, limit);
                        break;

                    case "CreatedDate-ASC":
                        products = await _context.Products.Include(p => p.Category).OrderBy(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;
                    case "CreatedDate-DESC":
                        products = await _context.Products.Include(p => p.Category).OrderByDescending(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;

                    case "UpdatedDate-ASC":
                        products = await _context.Products.Include(p => p.Category).OrderBy(x => x.Updated_Date).ToPagedListAsync(page, limit);
                        break;
                    case "UpdatedDate-DESC":
                        products = await _context.Products.Include(p => p.Category).OrderByDescending(x => x.Updated_Date).ToPagedListAsync(page, limit);
                        break;
                }

            }

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("userID");
            var userFound = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            var product = await _context.Products.Include(c => c.Category).Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == id);
            var reviewByProd = await _context.Reviews.Include(r => r.User).Where(x => x.ProductId == id).OrderByDescending(x => x.Created_Date).ToListAsync();
            ViewBag.userFound = null;
            if (userFound != null)
            {
                ViewBag.userFound = userFound;
                Console.WriteLine($"UserID Header: {userFound?.Id}");
            }
            ViewBag.product = product;
            ViewBag.reviewByProd = reviewByProd;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostComment(string previousUrl, [Bind("Id, Title, Subject, ProductId, UserId")] Review model)
        {
            if (HttpContext.Session.GetInt32("userID") != null)
            {
                if (ModelState.IsValid)
                {
                    model.Created_Date = DateTime.Now;
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    _toastNotification.Success("Post Comment Success !", 2);
                    return Redirect(previousUrl);
                }
                _toastNotification.Error("Post Comment Failed !", 2);
                return Redirect(previousUrl);
            }
            else
            {
                //return RedirectToAction("Login", "User");
                return RedirectToAction("Login", "User", new { returnUrl = previousUrl });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(string previousUrl, int Id)
        {
            if (HttpContext.Session.GetInt32("userID") != null)
            {
                if (ModelState.IsValid)
                {
                    var cmtFound = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == Id);
                    _context.Reviews.Remove(cmtFound);
                    await _context.SaveChangesAsync();
                    _toastNotification.Success("Delete Comment Success !", 2);
                    return Redirect(previousUrl);
                }
                _toastNotification.Error("Delete Comment Failed !", 2);
                return Redirect(previousUrl);
            }
            else
            {
                //return RedirectToAction("Login", "User");
                return RedirectToAction("Login", "User", new { returnUrl = previousUrl });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetComment(int id)
        {
            Console.WriteLine($"CMT Id:{id}");
            var cmtFound = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);

            return Json(cmtFound);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateComment(string previousUrl, [Bind("Id, Title, Subject, ProductId, UserId, Created_Date")] Review model)
        {
            if (HttpContext.Session.GetInt32("userID") != null)
            {
                if (ModelState.IsValid)
                {

                    model.Updated_Date = DateTime.Now;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    _toastNotification.Success("Update Comment Success !", 2);
                    return Redirect(previousUrl);
                }
                _toastNotification.Error("Update Comment Failed !", 2);
                return Redirect(previousUrl);
            }
            else
            {
                //return RedirectToAction("Login", "User");
                return RedirectToAction("Login", "User", new { returnUrl = previousUrl });
            }
        }
    }
}
