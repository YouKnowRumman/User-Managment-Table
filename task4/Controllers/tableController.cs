using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using table.Models;

namespace table.Controllers
{
    [Authorize] // Optional: requires login to see the table
    public class tableController : Controller
    {
        private readonly ApplicationDbContext _context;

        public tableController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: table
        public async Task<IActionResult> Index()
        {
            return View(await _context.Table.ToListAsync());
        }

        // GET: table/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userRecord = await _context.Table
                .FirstOrDefaultAsync(m => m.Id == id);

            if (userRecord == null) return NotFound();

            return View(userRecord);
        }

        // GET: table/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: table/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,email,status,lastSeen")] table.Models.Table userRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userRecord);
        }

        // GET: table/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userRecord = await _context.Table.FindAsync(id);
            if (userRecord == null) return NotFound();

            return View(userRecord);
        }

        // POST: table/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,email,status,lastSeen")] table.Models.Table userRecord)
        {
            if (id != userRecord.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!task4Exists(userRecord.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userRecord);
        }

        // GET: table/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userRecord = await _context.Table
                .FirstOrDefaultAsync(m => m.Id == id);

            if (userRecord == null) return NotFound();

            return View(userRecord);
        }

        // POST: table/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRecord = await _context.Table.FindAsync(id);
            if (userRecord != null)
            {
                _context.Table.Remove(userRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool task4Exists(int id)
        {
            return _context.Table.Any(e => e.Id == id);
        }
    }
}