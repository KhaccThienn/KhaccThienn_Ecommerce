using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KhaccThienn_Ecommerce.Data;
using KhaccThienn_Ecommerce.Models.DataModels;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace KhaccThienn_Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Role
        public async Task<IActionResult> Index(string? name, string? sort, int page = 1)
        {
            int limit = 3;
            page = page <= 1 ? 1 : page;
            var roles = _context.Roles.OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);

            if (!string.IsNullOrEmpty(name))
            {
                Console.WriteLine(name);

                ViewBag.names = name;
                roles = _context.Roles.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
            }
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;

                Console.WriteLine(sort);
                switch (sort)
                {
                    case "Id-ASC":
                        roles = _context.Roles.OrderBy(x => x.Id).ToPagedListAsync(page, limit);
                        break;
                    case "Id-DESC":
                        roles = _context.Roles.OrderByDescending(x => x.Id).ToPagedListAsync(page, limit);
                        break;

                    case "Name-ASC":
                        roles = _context.Roles.OrderBy(x => x.Name).ToPagedListAsync(page, limit);
                        break;
                    case "Name-DESC":
                        roles = _context.Roles.OrderByDescending(x => x.Name).ToPagedListAsync(page, limit);
                        break;

                    case "CreatedDate-ASC":
                        roles = _context.Roles.OrderBy(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;
                    case "CreatedDate-DESC":
                        roles = _context.Roles.OrderByDescending(x => x.Created_Date).ToPagedListAsync(page, limit);
                        break;

                    case "UpdatedDate-ASC":
                        roles = _context.Roles.OrderBy(x => x.Updated_Date).ToPagedListAsync(page, limit);
                        break;
                    case "UpdatedDate-DESC":
                        roles = _context.Roles.OrderByDescending(x => x.Updated_Date).ToPagedListAsync(page, limit);
                        break;
                }

            }
            return View(await roles);
        }

        // GET: Admin/Role/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Admin/Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Role/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Descriptions")] Role role)
        {
            if (_context.Roles.Any(x => x.Name == role.Name))
            {
                ModelState.AddModelError("Name",
                                  "The Role Name is already in use.");
            }
            if (ModelState.IsValid)
            {
                role.Created_Date = DateTime.Now;
                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Admin/Role/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Admin/Role/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Descriptions,Created_Date,Updated_Date")] Role role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            if (_context.Roles.Any(x => x.Name == role.Name && role.Id != id))
            {
                ModelState.AddModelError("Name",
                                  "This Role name is currently in use.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    role.Updated_Date = DateTime.Now;
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Admin/Role/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Admin/Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Roles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Roles'  is null.");
            }
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(int? id)
        {
            return (_context.Roles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
