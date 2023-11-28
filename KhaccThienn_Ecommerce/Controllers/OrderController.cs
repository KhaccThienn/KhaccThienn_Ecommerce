using AspNetCoreHero.ToastNotification.Abstractions;
using KhaccThienn_Ecommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaccThienn_Ecommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _toastNotification;
        public OrderController(ApplicationDbContext context, INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("userID") != null)
            {
                var userContext = HttpContext.User;
                var userId = HttpContext.Session.GetInt32("userID");
                Console.WriteLine($"UserId: {userId}");
                var userFound = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                var listOrderedByUser = await _context.Orders.Include(o => o.OrderDetails).Where(x => x.UserId == userId).ToListAsync();

                return View(listOrderedByUser);
            }
            else
            {
                //return RedirectToAction("Login", "User");
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderFound = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(x => x.Id == id);
            var orderDetails = await _context.OrderDetails.Include(od => od.Product).Where(x => x.OrderId == orderFound.Id).ToListAsync();
            ViewBag.orderDetails = null;
            if (orderDetails != null)
            {
                ViewBag.orderDetails = orderDetails;
            }
            return View(orderFound);
        }

        [HttpGet]
        public async Task<IActionResult> CancelOrder(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var orderFound = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (orderFound != null)
            {
                if (orderFound.Status == Enums.OrderStatuses.APPROVED)
                {
                    _toastNotification.Error("Cannot Delete Approved Order !", 3);
                    return RedirectToAction(nameof(Index));
                }
                if (orderFound.Status == Enums.OrderStatuses.CANCELED)
                {
                    _toastNotification.Warning("This Order Is Already Canceled !", 3);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    orderFound.Status = Enums.OrderStatuses.CANCELED;
                    _context.Orders.Update(orderFound);
                    await _context.SaveChangesAsync();
                    _toastNotification.Success($"Canceled Order #{id} !", 3);

                    return RedirectToAction(nameof(Index));
                }
            }
            _toastNotification.Error("Having An Error When Updating Order Status !", 3);
            return RedirectToAction(nameof(Index));
        }
    }
}
