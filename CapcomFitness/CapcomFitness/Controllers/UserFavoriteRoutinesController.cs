using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapcomFitness.Data;
using CapcomFitness.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CapcomFitness.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserFavoriteRoutinesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserFavoriteRoutinesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserFavoriteRoutines
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserFavoriteRoutines.Include(u => u.Routine).Include(u => u.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserFavoriteRoutines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserFavoriteRoutines == null)
            {
                return NotFound();
            }

            var userFavoriteRoutines = await _context.UserFavoriteRoutines
                .Include(u => u.Routine)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFavoriteRoutines == null)
            {
                return NotFound();
            }

            return View(userFavoriteRoutines);
        }

        // GET: UserFavoriteRoutines/Create
        public IActionResult Create()
        {
            ViewData["RoutineId"] = new SelectList(_context.Routines, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName");
            return View();
        }

        // POST: UserFavoriteRoutines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoutineId,UserId")] UserFavoriteRoutines userFavoriteRoutines)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userFavoriteRoutines);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoutineId"] = new SelectList(_context.Routines, "Id", "Name", userFavoriteRoutines.RoutineId);
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", userFavoriteRoutines.UserId);
            return View(userFavoriteRoutines);
        }

        // GET: UserFavoriteRoutines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserFavoriteRoutines == null)
            {
                return NotFound();
            }

            var userFavoriteRoutines = await _context.UserFavoriteRoutines.FindAsync(id);
            if (userFavoriteRoutines == null)
            {
                return NotFound();
            }
            ViewData["RoutineId"] = new SelectList(_context.Routines, "Id", "Name", userFavoriteRoutines.RoutineId);
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", userFavoriteRoutines.UserId);
            return View(userFavoriteRoutines);
        }

        // POST: UserFavoriteRoutines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoutineId,UserId")] UserFavoriteRoutines userFavoriteRoutines)
        {
            if (id != userFavoriteRoutines.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userFavoriteRoutines);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserFavoriteRoutinesExists(userFavoriteRoutines.Id))
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
            ViewData["RoutineId"] = new SelectList(_context.Routines, "Id", "Name", userFavoriteRoutines.RoutineId);
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", userFavoriteRoutines.UserId);
            return View(userFavoriteRoutines);
        }

        // GET: UserFavoriteRoutines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserFavoriteRoutines == null)
            {
                return NotFound();
            }

            var userFavoriteRoutines = await _context.UserFavoriteRoutines
                .Include(u => u.Routine)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFavoriteRoutines == null)
            {
                return NotFound();
            }

            return View(userFavoriteRoutines);
        }

        // POST: UserFavoriteRoutines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserFavoriteRoutines == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UserFavoriteRoutines'  is null.");
            }
            var userFavoriteRoutines = await _context.UserFavoriteRoutines.FindAsync(id);
            if (userFavoriteRoutines != null)
            {
                _context.UserFavoriteRoutines.Remove(userFavoriteRoutines);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserFavoriteRoutinesExists(int id)
        {
          return _context.UserFavoriteRoutines.Any(e => e.Id == id);
        }
    }
}
