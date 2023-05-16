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
    public class ExerciseExerciseTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExerciseExerciseTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExerciseExerciseTypes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ExerciseExerciseType.Include(e => e.Exercise).Include(e => e.ExerciseType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ExerciseExerciseTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ExerciseExerciseType == null)
            {
                return NotFound();
            }

            var exerciseExerciseType = await _context.ExerciseExerciseType
                .Include(e => e.Exercise)
                .Include(e => e.ExerciseType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exerciseExerciseType == null)
            {
                return NotFound();
            }

            return View(exerciseExerciseType);
        }

        // GET: ExerciseExerciseTypes/Create
        public IActionResult Create()
        {
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name");
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name");
            return View();
        }

        // POST: ExerciseExerciseTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExerciseId,ExerciseTypeId")] ExerciseExerciseType exerciseExerciseType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exerciseExerciseType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", exerciseExerciseType.ExerciseId);
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", exerciseExerciseType.ExerciseTypeId);
            return View(exerciseExerciseType);
        }

        // GET: ExerciseExerciseTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ExerciseExerciseType == null)
            {
                return NotFound();
            }

            var exerciseExerciseType = await _context.ExerciseExerciseType.FindAsync(id);
            if (exerciseExerciseType == null)
            {
                return NotFound();
            }
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", exerciseExerciseType.ExerciseId);
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", exerciseExerciseType.ExerciseTypeId);
            return View(exerciseExerciseType);
        }

        // POST: ExerciseExerciseTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExerciseId,ExerciseTypeId")] ExerciseExerciseType exerciseExerciseType)
        {
            if (id != exerciseExerciseType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exerciseExerciseType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseExerciseTypeExists(exerciseExerciseType.Id))
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
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", exerciseExerciseType.ExerciseId);
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", exerciseExerciseType.ExerciseTypeId);
            return View(exerciseExerciseType);
        }

        // GET: ExerciseExerciseTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ExerciseExerciseType == null)
            {
                return NotFound();
            }

            var exerciseExerciseType = await _context.ExerciseExerciseType
                .Include(e => e.Exercise)
                .Include(e => e.ExerciseType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exerciseExerciseType == null)
            {
                return NotFound();
            }

            return View(exerciseExerciseType);
        }

        // POST: ExerciseExerciseTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ExerciseExerciseType == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ExerciseExerciseType'  is null.");
            }
            var exerciseExerciseType = await _context.ExerciseExerciseType.FindAsync(id);
            if (exerciseExerciseType != null)
            {
                _context.ExerciseExerciseType.Remove(exerciseExerciseType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseExerciseTypeExists(int id)
        {
          return _context.ExerciseExerciseType.Any(e => e.Id == id);
        }
    }
}
