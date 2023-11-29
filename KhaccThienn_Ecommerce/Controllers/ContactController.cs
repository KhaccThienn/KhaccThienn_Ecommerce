using AspNetCoreHero.ToastNotification.Abstractions;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Models.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace KhaccThienn_Ecommerce.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _toastNotify;

        public ContactController(ApplicationDbContext context, INotyfService toastNotify)
        {
            _context = context;
            _toastNotify = toastNotify;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostContact([Bind("Id, Name, Email, Messsage")] Contact model)
        {
            if (ModelState.IsValid)
            {
                model.Created_Date = DateTime.Now;
                _context.Add(model);

                await _context.SaveChangesAsync(); 
                _toastNotify.Success("Post Message Success !", 3);
                return RedirectToAction("Index", "Home");

            }
            _toastNotify.Error("Post Message Failed !", 3) ;
            return RedirectToAction(nameof(Index));
        }
    }
}
