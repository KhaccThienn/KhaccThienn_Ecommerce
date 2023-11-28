using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Enums;
using KhaccThienn_Ecommerce.Models.DataModels;
using KhaccThienn_Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaccThienn_Ecommerce.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _toastNotification;
        private IMapper _mapper;

        public CheckoutController(ApplicationDbContext context, INotyfService toastNotification, IMapper mapper)
        {
            _context = context;
            _toastNotification = toastNotification;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (HttpContext.Session.GetInt32("userID") != null)
            {
                var userContext = HttpContext.User;
                var userId = HttpContext.Session.GetInt32("userID");
                Console.WriteLine($"UserId: {userId}");
                var userFound = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                var listCartByUser = await _context.Carts.Include(p => p.Product).Include(c => c.Product.Category).Include(a => a.User).Where(x => x.User.Id == userId).ToListAsync();
                ViewBag.listCartByUser = listCartByUser;

                decimal? subTotal = 0;
                foreach (var item in listCartByUser)
                {
                    subTotal += item.Total;
                }
                ViewBag.subTotal = subTotal;
                ViewBag.userFound = userFound;
                return View();
            }
            else
            {
                //return RedirectToAction("Login", "User");
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (HttpContext.Session.GetInt32("userID") != null)
            {
                var userContext = HttpContext.User;
                var userId = HttpContext.Session.GetInt32("userID");
                Console.WriteLine($"UserId: {userId}");
                var userFound = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                var listCartByUser = await _context.Carts.Include(p => p.Product).Include(c => c.Product.Category).Include(a => a.User).Where(x => x.User.Id == userId).ToListAsync();

                if (listCartByUser.Count == 0)
                {
                    _toastNotification.Warning("You must add the product to your cart first !", 3);
                    return RedirectToAction("Index", "Product");
                }


                if (ModelState.IsValid)
                {
                    decimal? totalPrice = 0;
                    foreach (var i in listCartByUser)
                    {
                        totalPrice += i.Total;
                    }
                    model.UserId = userFound?.Id;
                    model.Created_Date = DateTime.Now;
                    model.TotalPrice = totalPrice;
                    model.Status = (int?)OrderStatuses.PROCESSING;
                    var orderMapper = _mapper.Map<Order>(model);

                    var orderDb = await _context.Orders.AddAsync(orderMapper);
                    await _context.SaveChangesAsync();


                    foreach (var item in listCartByUser)
                    {
                        var details = new OrderDetailsViewModel
                        {
                            OrderId = orderDb.Entity.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            TotalPrice = item.Total
                        };
                        var orderDetailsMapper = _mapper.Map<OrderDetail>(details);
                        _context.OrderDetails.Add(orderDetailsMapper);
                    }

                    _context.Carts.RemoveRange(listCartByUser);
                    await _context.SaveChangesAsync();
                    _toastNotification.Success("Checkout Successfully !", 3);
                    return RedirectToAction("Index", "Order");

                }

                _toastNotification.Error("Having An Error When Checking Out !", 3);
                decimal? subTotal = 0;
                foreach (var item in listCartByUser)
                {
                    subTotal += item.Total;
                }
                ViewBag.subTotal = subTotal;
                ViewBag.listCartByUser = listCartByUser;
                ViewBag.userFound = userFound;
                return View(model);
            }
            else
            {
                //return RedirectToAction("Login", "User");
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }

        }


    }
}
