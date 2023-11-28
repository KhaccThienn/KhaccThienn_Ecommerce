using Microsoft.AspNetCore.Mvc;
using KhaccThienn_Ecommerce.Models;
using KhaccThienn_Ecommerce.Data;
using System.Collections.Generic;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

namespace KhaccThienn_Ecommerce.ViewComponents
{
    public class CustomersHeaderViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CustomersHeaderViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var categories = _context.Categories.Where(x => x.Status == 1).ToList();
            var userId = HttpContext.Session.GetInt32("userID");

            var userFound = _context.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.userFound = null;
            if (userFound != null)
            {
                ViewBag.userFound = userFound;
                Console.WriteLine($"UserID Header: {userFound?.Id}");
            }
           

            return View(categories);
        }
    }
}
