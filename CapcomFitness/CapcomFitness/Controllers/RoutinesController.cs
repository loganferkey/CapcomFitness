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
using CapcomFitness.ViewModels;
using CapcomFitness.Utility;
using Microsoft.Net.Http.Headers;

namespace CapcomFitness.Controllers
{
    public class RoutinesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtility _utility;

        public RoutinesController(ApplicationDbContext context, IUtility utility)
        {
            _context = context;
            _utility = utility;
        }

        // GET: Routines
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Routines.Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> UserRoutines(int? pageNumber, string tab, string sortBy, string searchString)
        {
            ViewBag.SortBy = sortBy;
            // Need the user and favorite routines for that user for correct display
            AppUser currentUser = await _utility.getHTTPContextUser();
            List<UserFavoriteRoutines>? userFavoriteRoutines = null;

            if (currentUser != null)
            {
                        userFavoriteRoutines = await _context.UserFavoriteRoutines
                        .Where(ufr => ufr.UserId == currentUser.Id)
                        .Include(ufr => ufr.Routine)
                        .ToListAsync();
            }
                
            RoutinesViewModel rvm = new RoutinesViewModel
            {
                Tab = (tab != null) ? tab : "routines",
                User = currentUser
            };

            rvm.Routines = null;
            rvm.FavoriteRoutines = null;

            switch (tab)
            {
                case "favorites":
                    if (currentUser != null && userFavoriteRoutines.Count() > 0)
                    {
                        var userFavoriteRoutineList = userFavoriteRoutines
                            .Where(r => String.IsNullOrEmpty(searchString) || r.Routine!.Name.ToUpper().Contains(searchString.ToUpper()))
                            .Select(r => { r.Routine!.UserFavorited = true; return r.Routine; })
                            .ToList();

                        switch (sortBy)
                        {
                            case "name_desc":
                                userFavoriteRoutineList = userFavoriteRoutineList.OrderByDescending(r => r.Name).ToList();
                                break;
                            case "name_asc":
                                userFavoriteRoutineList = userFavoriteRoutineList.OrderBy(r => r.Name).ToList();
                                break;
                        }

                        if (userFavoriteRoutineList.Count() > 0)
                        {
                            rvm.FavoriteRoutines = await PaginatedList<Routines>.CreateAsync(userFavoriteRoutineList, pageNumber ?? 1, 8);
                        }
                    }
                    break;
                case "created":
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        var routines = _context.Routines.Include(r => r.User).Where(r => r.Name.Contains(searchString)).Where(r => r.User == currentUser);

                        if (routines.Count() > 0)
                        {
                            rvm.Routines = await PaginatedList<Routines>.CreateAsync(routines, pageNumber ?? 1, 8);

                        }
                    } else
                    {
                        switch (sortBy)
                        {
                            case "name_desc":
                                var routines = _context.Routines.Include(r => r.User).AsNoTracking().Where(r => r.User == currentUser).OrderByDescending(r => r.Name);
                                if (routines.Count() > 0)
                                {
                                    rvm.Routines = await PaginatedList<Routines>.CreateAsync(routines, pageNumber ?? 1, 8);

                                }
                                break;
                            case "name_asc":
                                routines = _context.Routines.Include(r => r.User).AsNoTracking().Where(r => r.User == currentUser).OrderBy(r => r.Name);
                                if (routines.Count() > 0)
                                {
                                    rvm.Routines = await PaginatedList<Routines>.CreateAsync(routines, pageNumber ?? 1, 8);

                                }
                                break;
                            default:
                                routines = _context.Routines.Include(r => r.User).AsNoTracking().Where(r => r.User == currentUser).OrderBy(r => r.Name);
                                if (routines.Count() > 0)
                                {
                                    rvm.Routines = await PaginatedList<Routines>.CreateAsync(routines, pageNumber ?? 1, 8);
                                }
                                break;
                        }
                    }

                    _utility.determineFavoriteRoutines(rvm.Routines, userFavoriteRoutines);
                    break;
                default:
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        var routines = _context.Routines.Include(r => r.User).Where(r => r.Name.Contains(searchString));

                        if (routines.Count() > 0)
                        {
                            rvm.Routines = await PaginatedList<Routines>.CreateAsync(routines, pageNumber ?? 1, 8);

                        }
                    } else
                    {
                        switch (sortBy)
                        {
                            case "name_desc":
                                var routines = _context.Routines.Include(r => r.User).AsNoTracking().OrderByDescending(r => r.Name);
                                if (routines.Count() > 0)
                                {
                                    rvm.Routines = await PaginatedList<Routines>.CreateAsync(routines, pageNumber ?? 1, 8);

                                }
                                break;
                            case "name_asc":
                                routines = _context.Routines.Include(r => r.User).AsNoTracking().OrderBy(e => e.Name);
                                if (routines.Count() > 0)
                                {
                                    rvm.Routines = await PaginatedList<Routines>.CreateAsync(routines, pageNumber ?? 1, 8);
                                }
                                break;
                            default:
                                if (_context.Routines.Count() > 0)
                                {
                                    rvm.Routines = await PaginatedList<Routines>.CreateAsync(_context.Routines.Include(r => r.User).AsNoTracking(), pageNumber ?? 1, 8);
                                } else
                                {
                                    rvm.Routines = null;
                                }
                                
                                break;
                        }
                    }

                    _utility.determineFavoriteRoutines(rvm.Routines, userFavoriteRoutines);
                    break;
            }

            return View(rvm);
        }

        // This is simply an action to create a routine with no page attached, it redirects to other actions after adding the routine
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserCreateRoutine(string? Name, string? Description, bool Private, string? tab, string? search)
        {
            // Modelstate wasn't working properly on this form when private isn't selected
            if (Name != null)
            {
                AppUser user = await _utility.getHTTPContextUser();
                // If the routine is valid add it to the database
                try
                {
                    // User shouldn't be null here with the authorize attribute, if they are they'll be redirected to login page
                    Routines routine = new Routines
                    {
                        Name = Name,
                        Description = Description,
                        Private = Private,
                        UserId = user!.Id
                    };
                    _context.Routines.Add(routine);
                    await _context.SaveChangesAsync();
                    // Instead of by Id I think I'll have it search by name so it looks better in the URL as well
                    return RedirectToAction("UserRoutinesDetail", new { id = routine.Name });
                }
                catch (Exception ex)
                {
                    // Redirect with the error once we have toast notifications on merge
                    return RedirectToAction("UserRoutines", new { tab = tab, search = search });
                }

            }
            else
            {
                return RedirectToAction("UserRoutines", new { tab = tab, search = search });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserRoutinesDetail(string? id)
        {
            RoutineDetailViewModel rdvm = new RoutineDetailViewModel();
            // TODO: Maybe filter out all exercises the routine already has, however I don't know if we should allow duplicate exercises or not
            ViewData["Exercises"] = new SelectList(await _context.Exercises.ToListAsync(), "Id", "Name");

            // Grab the current user or null
            rdvm.User = await _utility.getHTTPContextUser();

            if (id != null)
            {
                // Match the name find the routine
                rdvm.Routine = await _context.Routines.Where(r => r.Name.ToLower().Trim() == id.ToLower().Trim()).Include(r => r.User).FirstOrDefaultAsync();
                if (rdvm.Routine != null)
                {
                    // If there was an actual routine, pass any exercises it has as well
                    rdvm.RoutineExercises = await _context.RoutineExercises.Where(re => re.RoutineId == rdvm.Routine.Id).Include(re => re.Exercise).Include(re => re.Routine).ToListAsync();
                }
            }

            // Return the view with the found routine and accessories
            return View(rdvm);
        }

        // GET: Routines/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Routines == null)
            {
                return NotFound();
            }

            var routines = await _context.Routines
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (routines == null)
            {
                return NotFound();
            }

            return View(routines);
        }

        // GET: Routines/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName");
            return View();
        }

        // POST: Routines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Private,UserId")] Routines routines)
        {
            if (ModelState.IsValid)
            {
                _context.Add(routines);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", routines.UserId);
            return View(routines);
        }

        // GET: Routines/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Routines == null)
            {
                return NotFound();
            }

            var routines = await _context.Routines.FindAsync(id);
            if (routines == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", routines.UserId);
            return View(routines);
        }

        // POST: Routines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Private,UserId")] Routines routines)
        {
            if (id != routines.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routines);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoutinesExists(routines.Id))
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
            ViewData["UserId"] = new SelectList(_context.AppUser, "Id", "UserName", routines.UserId);
            return View(routines);
        }

        // GET: Routines/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Routines == null)
            {
                return NotFound();
            }

            var routines = await _context.Routines
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (routines == null)
            {
                return NotFound();
            }

            return View(routines);
        }

        // POST: Routines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Routines == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Routines'  is null.");
            }
            var routines = await _context.Routines.FindAsync(id);
            if (routines != null)
            {
                _context.Routines.Remove(routines);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoutinesExists(int id)
        {
          return _context.Routines.Any(e => e.Id == id);
        }
    }
}
