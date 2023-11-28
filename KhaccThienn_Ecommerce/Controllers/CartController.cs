using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Helpers;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KhaccThienn_Ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _toastNotification;

        public CartController(INotyfService toastNotification, ApplicationDbContext context)
        {
            _toastNotification = toastNotification;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("userID") != null)
            {
                var userContext = HttpContext.User;
                var userId = HttpContext.Session.GetInt32("userID");
                Console.WriteLine($"UserId: {userId}");

                var listCartByUser = await _context.Carts.Include(p => p.Product).Include(a => a.User).Where(x => x.User.Id == userId).ToListAsync();
                return View(listCartByUser);
            }
            else
            {
                //return RedirectToAction("Login", "User");
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }
            
        }

        public async Task<IActionResult> AddToCart(int? accId, int prodId, int? quantity)
        {
            if (HttpContext.Session.GetInt32("userID") != null)
            {
                accId = HttpContext.Session.GetInt32("userID");

                Console.WriteLine($"Acc Id: {accId}");
                Console.WriteLine($"Prod Id: {prodId}");
                Console.WriteLine($"Prod Quantity: {quantity}");

                var prodFound = await _context.Products.FirstOrDefaultAsync(x => x.Id == prodId);

                var prodFoundInCart = await _context.Carts.FirstOrDefaultAsync(x => x.ProductId == prodId && x.User.Id == accId);

                if (prodFoundInCart != null)
                {
                    prodFoundInCart.Quantity += quantity != null ? quantity : 1;
                    prodFoundInCart.Total += prodFound?.SalePrice > 0 ? prodFound?.SalePrice * quantity : prodFound?.Price * quantity;
                    _context.Carts.Update(prodFoundInCart);
                }
                else
                {
                    var cart = new Cart()
                    {
                        Quantity = quantity != null ? quantity : 1,
                        Total = prodFound?.SalePrice > 0 ? prodFound?.SalePrice * quantity : prodFound?.Price * quantity,
                        ProductId = prodId,
                        UserId = accId,
                        Created_Date = DateTime.Now,
                    };

                    _context.Carts.Add(cart);
                }

                await _context.SaveChangesAsync();

                _toastNotification.Success("Add To Cart Successfully !", 3);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //return RedirectToAction("Login", "User");
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }
           
        }

        public async Task<IActionResult> Update(int Id, int ProdId, int quantity)
        {
            Console.WriteLine($"CartID: {Id}");
            Console.WriteLine($"ProdId: {ProdId}");
            Console.WriteLine($"Cart Quantity: {quantity}");

            var prodFoundInCart = await _context.Carts.FirstOrDefaultAsync(x => x.ID == Id);
            var prodFound = await _context.Products.FirstOrDefaultAsync(x => x.Id == ProdId);
            if (prodFoundInCart != null)
            {
                prodFoundInCart.Quantity = quantity;
                prodFoundInCart.Total = prodFound?.SalePrice > 0 ? prodFound?.SalePrice * quantity : prodFound?.Price * quantity;
                _context.Carts.Update(prodFoundInCart);
                await _context.SaveChangesAsync();
                _toastNotification.Success("Update Cart Successfully !", 3);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }
            var cartFound = await _context.Carts.FirstOrDefaultAsync(x => x.ID == id);
            if(cartFound != null)
            {
                _context.Remove(cartFound);
                await _context.SaveChangesAsync();
                _toastNotification.Success("Delete Cart Item Successfully !", 3);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
