using AspNetCoreHero.ToastNotification.Abstractions;
using KhaccThienn_Ecommerce.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using X.PagedList;

namespace KhaccThienn_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
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
        public async Task<IActionResult> Index(string? sort, int page = 1)
        {
            page = page <= 1 ? 1 : page;
            int limit = 5;
            var listOrderPaginated = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).ToPagedListAsync(page, limit);
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;

                Console.WriteLine(sort);
                switch (sort)
                {
                    case "Id-ASC":
                        listOrderPaginated = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).OrderBy(x => x.Id).ToPagedListAsync(page, limit);
                        break;
                    case "Id-DESC":
                        listOrderPaginated = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).ToPagedListAsync(page, limit);
                        break;

                    case "Created_Date-ASC":
                        listOrderPaginated = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).OrderBy(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;
                    case "Created_Date-DESC":
                        listOrderPaginated = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).OrderByDescending(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;

                    case "Status-ASC":
                        listOrderPaginated = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).OrderBy(x => x.Status).ToPagedListAsync(page, limit);
                        break;
                    case "Status-DESC":
                        listOrderPaginated = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).OrderByDescending(x => x.Status).ToPagedListAsync(page, limit);
                        break;
                }

            }
            return View(listOrderPaginated);
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
        public async Task<IActionResult> ApproveOrder(int? id)
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
                    _toastNotification.Warning("This Order Already Approved !", 3);
                    return RedirectToAction(nameof(Index));
                }

                if (orderFound.Status == Enums.OrderStatuses.CANCELED)
                {
                    _toastNotification.Error("This Order Is Already Canceled, Cannot Approve !", 3);
                    return RedirectToAction(nameof(Index));
                }

                else
                {
                    orderFound.Status = Enums.OrderStatuses.APPROVED;
                    _context.Orders.Update(orderFound);
                    await _context.SaveChangesAsync();
                    _toastNotification.Success($"Approved Order #{id} !", 3);

                    return RedirectToAction(nameof(Index));
                }
            }
            _toastNotification.Error("Having An Error When Updating Order Status", 3);
            return RedirectToAction(nameof(Index));
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
