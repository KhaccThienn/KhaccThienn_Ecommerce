using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Math;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using X.PagedList;

namespace KhaccThienn_Ecommerce.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ContactController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly INotyfService _toastNotification;
        public ContactController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, INotyfService notyfService)
        {
            _context = context;
            _environment = environment;
            _toastNotification = notyfService;
        }
        [HttpGet]
        public async Task<IActionResult> Index( string? sort, int? page = 1)
        {
            int limit = 3;
            page = page <= 1 ? 1 : page;
            ViewBag.sorts = sort;

            var contacts = _context.Contacts.OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
            
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;

                Console.WriteLine(sort);
                switch (sort)
                {
                    case "Id-ASC":
                        contacts = _context.Contacts.OrderBy(x => x.Id).ToPagedListAsync(page, limit);
                        break;
                    case "Id-DESC":
                        contacts = _context.Contacts.OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
                        break;

                    case "CreatedDate-ASC":
                        contacts = _context.Contacts.OrderBy(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;
                    case "CreatedDate-DESC":
                        contacts = _context.Contacts.OrderByDescending(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;
                }

            }
            return View(await contacts);
        }

        [HttpGet]
        public async Task<FileResult> Export()
        {
            var contacts = await _context.Contacts.OrderByDescending(x => x.Id).ToListAsync();

            var fileName = "contactData.xlsx";

            return GenerateExcel(fileName, contacts);
        }

        private FileResult GenerateExcel(string fileName, IEnumerable<Contact> contacts)
        {
            DataTable dataTable = new DataTable("Contacts");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("ID"),
                new DataColumn("Name"),
                new DataColumn("Email"),
                new DataColumn("Message"),
                new DataColumn("Created Date"),
            });

            foreach(var contact in contacts)
            {
                dataTable.Rows.Add(contact.Id, contact.Name, contact.Email, contact.Messsage, contact.Created_Date);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);

                using(MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Contacts'  is null.");
            }

            var reviewFound = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == id);
            if (reviewFound != null)
            {
                _context.Remove(reviewFound);
                _toastNotification.Success("Removed Contact Information", 3);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            _toastNotification.Error("Remove Contact Information Failed", 3);            

            return RedirectToAction(nameof(Index));
        }
    }
}
