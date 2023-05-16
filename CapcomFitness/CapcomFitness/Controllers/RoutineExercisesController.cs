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
    public class RoutineExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoutineExercisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RoutineExercises
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RoutineExercises.Include(r => r.Exercise).Include(r => r.Routine);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RoutineExercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RoutineExercises == null)
            {
                return NotFound();
            }

            var routineExercises = await _context.RoutineExercises
                .Include(r => r.Exercise)
                .Include(r => r.Routine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (routineExercises == null)
            {
                return NotFound();
            }

            return View(routineExercises);
        }

        // GET: RoutineExercises/Create
        public IActionResult Create()
        {
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name");
            ViewData["RoutineId"] = new SelectList(_context.Routines, "Id", "Name");
            return View();
        }

        // POST: RoutineExercises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sets,Reps,AdditionalInfo,RoutineId,ExerciseId")] RoutineExercises routineExercises)
        {
            if (ModelState.IsValid)
            {
                _context.Add(routineExercises);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", routineExercises.ExerciseId);
            ViewData["RoutineId"] = new SelectList(_context.Routines, "Id", "Name", routineExercises.RoutineId);
            return View(routineExercises);
        }

        // GET: RoutineExercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RoutineExercises == null)
            {
                return NotFound();
            }

            var routineExercises = await _context.RoutineExercises.FindAsync(id);
            if (routineExercises == null)
            {
                return NotFound();
            }
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", routineExercises.ExerciseId);
            ViewData["RoutineId"] = new SelectList(_context.Routines, "Id", "Name", routineExercises.RoutineId);
            return View(routineExercises);
        }

        // POST: RoutineExercises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Sets,Reps,AdditionalInfo,RoutineId,ExerciseId")] RoutineExercises routineExercises)
        {
            if (id != routineExercises.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routineExercises);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoutineExercisesExists(routineExercises.Id))
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
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", routineExercises.ExerciseId);
            ViewData["RoutineId"] = new SelectList(_context.Routines, "Id", "Name", routineExercises.RoutineId);
            return View(routineExercises);
        }

        // GET: RoutineExercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RoutineExercises == null)
            {
                return NotFound();
            }

            var routineExercises = await _context.RoutineExercises
                .Include(r => r.Exercise)
                .Include(r => r.Routine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (routineExercises == null)
            {
                return NotFound();
            }

            return View(routineExercises);
        }

        // POST: RoutineExercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RoutineExercises == null)
            {
                return Problem("Entity set 'ApplicationDbContext.RoutineExercises'  is null.");
            }
            var routineExercises = await _context.RoutineExercises.FindAsync(id);
            if (routineExercises != null)
            {
                _context.RoutineExercises.Remove(routineExercises);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoutineExercisesExists(int id)
        {
          return _context.RoutineExercises.Any(e => e.Id == id);
        }
    }
}
