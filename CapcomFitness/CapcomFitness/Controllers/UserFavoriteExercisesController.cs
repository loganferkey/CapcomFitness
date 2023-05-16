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
    public class UserFavoriteExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserFavoriteExercisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserFavoriteExercises
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserFavoriteExercises.Include(u => u.Exercise).Include(u => u.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UserFavoriteExercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserFavoriteExercises == null)
            {
                return NotFound();
            }

            var userFavoriteExercises = await _context.UserFavoriteExercises
                .Include(u => u.Exercise)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFavoriteExercises == null)
            {
                return NotFound();
            }

            return View(userFavoriteExercises);
        }

        // GET: UserFavoriteExercises/Create
        public IActionResult Create()
        {
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName");
            return View();
        }

        // POST: UserFavoriteExercises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExerciseId,UserId")] UserFavoriteExercises userFavoriteExercises)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userFavoriteExercises);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", userFavoriteExercises.ExerciseId);
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", userFavoriteExercises.UserId);
            return View(userFavoriteExercises);
        }

        // GET: UserFavoriteExercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserFavoriteExercises == null)
            {
                return NotFound();
            }

            var userFavoriteExercises = await _context.UserFavoriteExercises.FindAsync(id);
            if (userFavoriteExercises == null)
            {
                return NotFound();
            }
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", userFavoriteExercises.ExerciseId);
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", userFavoriteExercises.UserId);
            return View(userFavoriteExercises);
        }

        // POST: UserFavoriteExercises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExerciseId,UserId")] UserFavoriteExercises userFavoriteExercises)
        {
            if (id != userFavoriteExercises.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userFavoriteExercises);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserFavoriteExercisesExists(userFavoriteExercises.Id))
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
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", userFavoriteExercises.ExerciseId);
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", userFavoriteExercises.UserId);
            return View(userFavoriteExercises);
        }

        // GET: UserFavoriteExercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserFavoriteExercises == null)
            {
                return NotFound();
            }

            var userFavoriteExercises = await _context.UserFavoriteExercises
                .Include(u => u.Exercise)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFavoriteExercises == null)
            {
                return NotFound();
            }

            return View(userFavoriteExercises);
        }

        // POST: UserFavoriteExercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserFavoriteExercises == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UserFavoriteExercises'  is null.");
            }
            var userFavoriteExercises = await _context.UserFavoriteExercises.FindAsync(id);
            if (userFavoriteExercises != null)
            {
                _context.UserFavoriteExercises.Remove(userFavoriteExercises);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserFavoriteExercisesExists(int id)
        {
          return _context.UserFavoriteExercises.Any(e => e.Id == id);
        }
    }
}
