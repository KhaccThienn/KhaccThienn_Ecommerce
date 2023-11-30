using AspNetCoreHero.ToastNotification.Abstractions;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [HttpGet]
        public IActionResult Index()
        {
            GetRemoteHostIpAddress(HttpContext);
            var orderCount = _context.Orders.Count();
            var productCount = _context.Products.Count();
            var userCount = _context.Users.Count();
            var onlineUser = UserIpTracker.GetUserCount();
            _toastNotification.Success($"Welcome Back, {User.Identity.Name.ToString()}", 3);

            ViewBag.orderCount = orderCount;
            ViewBag.productCount = productCount;
            ViewBag.userCount = userCount;
            ViewBag.onlineUser = onlineUser;

            return View();
        }


        public void GetRemoteHostIpAddress(HttpContext context)
        {
            IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
            string ipv4 = "";
            if (remoteIpAddress != null)
            {
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }

                ipv4 = remoteIpAddress.ToString();

                // Theo dõi địa chỉ IP của người dùng
                UserIpTracker.TrackUserIp(ipv4);
            }
        }

    }
}
