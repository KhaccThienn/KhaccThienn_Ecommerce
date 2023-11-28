using Microsoft.AspNetCore.Mvc;

namespace KhaccThienn_Ecommerce.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
