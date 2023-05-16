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

namespace CapcomFitness.Controllers
{
    [Authorize(Roles = "admin")]
    public class ExerciseTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExerciseTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExerciseTypes
        public async Task<IActionResult> Index()
        {
              return View(await _context.ExerciseTypes.ToListAsync());
        }

        // GET: ExerciseTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ExerciseTypes == null)
            {
                return NotFound();
            }

            var exerciseTypes = await _context.ExerciseTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exerciseTypes == null)
            {
                return NotFound();
            }

            return View(exerciseTypes);
        }

        // GET: ExerciseTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExerciseTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] ExerciseTypes exerciseTypes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exerciseTypes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exerciseTypes);
        }

        // GET: ExerciseTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ExerciseTypes == null)
            {
                return NotFound();
            }

            var exerciseTypes = await _context.ExerciseTypes.FindAsync(id);
            if (exerciseTypes == null)
            {
                return NotFound();
            }
            return View(exerciseTypes);
        }

        // POST: ExerciseTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] ExerciseTypes exerciseTypes)
        {
            if (id != exerciseTypes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exerciseTypes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseTypesExists(exerciseTypes.Id))
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
            return View(exerciseTypes);
        }

        // GET: ExerciseTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ExerciseTypes == null)
            {
                return NotFound();
            }

            var exerciseTypes = await _context.ExerciseTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exerciseTypes == null)
            {
                return NotFound();
            }

            return View(exerciseTypes);
        }

        // POST: ExerciseTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ExerciseTypes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ExerciseTypes'  is null.");
            }
            var exerciseTypes = await _context.ExerciseTypes.FindAsync(id);
            if (exerciseTypes != null)
            {
                _context.ExerciseTypes.Remove(exerciseTypes);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseTypesExists(int id)
        {
          return _context.ExerciseTypes.Any(e => e.Id == id);
        }
    }
}
