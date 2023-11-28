using AspNetCoreHero.ToastNotification.Abstractions;
using KhaccThienn_Ecommerce.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhaccThienn_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Route("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _toastNotification;
        public HomeController(ApplicationDbContext context, INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            _toastNotification.Success($"Welcome Back, {User.Identity.Name.ToString()}", 3);
            return View();
        }
    }
}
