using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KhaccThienn_Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("userID");

            var userFound = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            ViewBag.userFound = null;
            if (userFound != null)
            {
                ViewBag.userFound = userFound;
                Console.WriteLine($"UserID Header: {userFound?.Id}");
            }
            var categories = _context.Categories.Include(c => c.Products).Where(x => x.Status == 1).OrderByDescending(x => x.Updated_Date).ToListAsync();
            return View(await categories);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}