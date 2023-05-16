using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapcomFitness.Data;
using CapcomFitness.Models;
using CapcomFitness.Utility;
using Microsoft.AspNetCore.Authorization;
using CapcomFitness.ViewModels;

namespace CapcomFitness.Controllers
{
    // Whoever makes the weight history action view for users will have to put [Authorize(Roles = "user,admin")] above their view action!
    public class WeightHistoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtility _utility;

        public WeightHistoriesController(ApplicationDbContext context, IUtility utility)
        {
            _context = context;
            _utility = utility;
        }

        // GET: WeightHistories
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WeightHistory.Include(w => w.User);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> Tracker(int? pageNumber, string sortBy)
        {
			ViewBag.SortBy = sortBy;

			// Get the current logged in user, and all their weight histories, and pass them to view!
			AppUser user = await _utility.getHTTPContextUser();

            WeightHistoryViewModel whvm = new WeightHistoryViewModel
            {
                User = user
            };

            switch (sortBy)
            {
                case "date_desc":
                    var weightHistory = _context.WeightHistory.AsNoTracking().Where(w => w.UserId == user.Id).OrderByDescending(wh => wh.DateEntered);

                    if (weightHistory.Count() > 0)
                    {
                        whvm.WeightHistoriesPaginated = await PaginatedList<WeightHistory>.CreateAsync(weightHistory, pageNumber ?? 1, 5);
                        whvm.WeightHistories = weightHistory.ToList();

                    }
                    break;
                case "date_asc":
                    weightHistory = _context.WeightHistory.AsNoTracking().Where(w => w.UserId == user.Id).OrderBy(wh => wh.DateEntered);

                    if (weightHistory.Count() > 0)
                    {
                        whvm.WeightHistoriesPaginated = await PaginatedList<WeightHistory>.CreateAsync(weightHistory, pageNumber ?? 1, 5);
                        whvm.WeightHistories = weightHistory.ToList();
                    }
                    break;
                default:
                    weightHistory = _context.WeightHistory.AsNoTracking().Where(w => w.UserId == user.Id).OrderByDescending(wh => wh.DateEntered);

                    if (_context.WeightHistory.Any(w => w.UserId == user.Id))
                    {
                        whvm.WeightHistoriesPaginated = await PaginatedList<WeightHistory>.CreateAsync(weightHistory, pageNumber ?? 1, 5);
                        whvm.WeightHistories = weightHistory.ToList();

                    }
                    break;
            }

            return View(whvm);
        }

        // GET: WeightHistories/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WeightHistory == null)
            {
                return NotFound();
            }

            var weightHistory = await _context.WeightHistory
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weightHistory == null)
            {
                return NotFound();
            }

            return View(weightHistory);
        }

        // GET: WeightHistories/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName");
            return View();
        }

        // POST: WeightHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Id,Weight,Height,DateEntered,UserId")] WeightHistory weightHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(weightHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", weightHistory.UserId);
            return View(weightHistory);
        }

        // GET: WeightHistories/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WeightHistory == null)
            {
                return NotFound();
            }

            var weightHistory = await _context.WeightHistory.FindAsync(id);
            if (weightHistory == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", weightHistory.UserId);
            return View(weightHistory);
        }

        // POST: WeightHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Weight,Height,DateEntered,UserId")] WeightHistory weightHistory)
        {
            if (id != weightHistory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weightHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeightHistoryExists(weightHistory.Id))
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
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", weightHistory.UserId);
            return View(weightHistory);
        }

        // GET: WeightHistories/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WeightHistory == null)
            {
                return NotFound();
            }

            var weightHistory = await _context.WeightHistory
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weightHistory == null)
            {
                return NotFound();
            }

            return View(weightHistory);
        }

        // POST: WeightHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WeightHistory == null)
            {
                return Problem("Entity set 'ApplicationDbContext.WeightHistory'  is null.");
            }
            var weightHistory = await _context.WeightHistory.FindAsync(id);
            if (weightHistory != null)
            {
                _context.WeightHistory.Remove(weightHistory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeightHistoryExists(int id)
        {
          return _context.WeightHistory.Any(e => e.Id == id);
        }
    }
}
