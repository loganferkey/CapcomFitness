using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapcomFitness.Data;
using CapcomFitness.Models;
using System.Drawing.Text;
using CapcomFitness.Utility;
using CapcomFitness.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.Intrinsics.Arm;
using NuGet.Packaging.Licenses;
using NuGet.Protocol.Core.Types;
using Microsoft.Build.Framework;

namespace CapcomFitness.Controllers
{
    [Authorize(Roles = "admin")]
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUtility _utility;

        public ExercisesController(ApplicationDbContext context, IUtility utility)
        {
            _context = context;
            _utility = utility;
        }

        // GET: Exercises
        public async Task<IActionResult> Index()
        {
              return View(await _context.Exercises.ToListAsync());
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> UserExercises(int? pageNumber, string tab, string sortBy, string searchString)
        {
            // Sets the sort by view bag.
            ViewBag.SortBy = sortBy;
            // Gets the current user.
            AppUser currentUser = await _utility.getHTTPContextUser();
            // Instantiates a new list of Favorite Exercises.
            List<UserFavoriteExercises> userFavoriteExercises = null;
            // Gets all the exercise types.
            List<ExerciseTypes> exerciseTypes = await _context.ExerciseTypes.ToListAsync();

            // Instantiates a new ExercisesViewModel.
            ExercisesViewModel evm = new ExercisesViewModel
            {
                Tab = (tab != null) ? tab : "exercises",
                User = currentUser,
                ExerciseTypes = exerciseTypes
            };

            // Sets the exercises and favorite exercises to null.
            evm.Exercises = null;
            evm.FavoriteExercises = null;

            // If the current user is not null, get all the exercises that the user has favorited.
            if (currentUser != null)
            {
                userFavoriteExercises = await _context.UserFavoriteExercises
                .Where(ufe => ufe.UserId == currentUser.Id)
                .Include(e => e.Exercise)
                .ToListAsync();
            }

            // Switch on the tab that is passed in.
            switch (tab)
            {
                // If the tab is favorites, get all the favorite exercises.
                case "favorites":
                    // If the current user is not null and the user has favorited exercises, get the exercises.
                    if (currentUser != null && userFavoriteExercises.Count() > 0)
                    {
                        var userFavoriteExerciseList = userFavoriteExercises
                            .Where(e => String.IsNullOrEmpty(searchString) || e.Exercise!.Name.ToUpper().Contains(searchString.ToUpper()))
                            .Select(e => { e.Exercise!.UserFavorited = true; return e.Exercise!; })
                            .ToList();

                        // Switch on the sortby parameter.
                        switch (sortBy)
                        {
                            // If the sortby parameter is name_desc, order the exercises by name descending.
                            case "name_desc":
                                userFavoriteExerciseList = userFavoriteExerciseList.OrderByDescending(e => e.Name).ToList();
                                break;
                            // If the sortby parameter is name_asc, order the exercises by name ascending.
                            case "name_asc":
                                userFavoriteExerciseList = userFavoriteExerciseList.OrderBy(e => e.Name).ToList();
                                break;
                            // If the sortBy parameter is Cardio, order the exercises by Cardio.
                            case "Cardio":
                                // Loop through the exercise types.
                                foreach (var exType in exerciseTypes)
                                {
                                    // If the exercise type is Cardio, get the exercises that are Cardio.
                                    if (exType.Name == "Cardio")
                                    {
                                        userFavoriteExerciseList = _context.UserFavoriteExercises
                                        .Where(e => e.UserId == currentUser.Id && e.Exercise.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                                        .Include(e => e.Exercise.ExerciseExerciseType)
                                        .Select(e => e.Exercise)
                                        .ToList();

                                        // If the user has favorited exercises, set the View Models FavoriteExercises to a paginated list of the exercises.
                                        if (userFavoriteExerciseList.Count() > 0)
                                        {
                                            evm.FavoriteExercises = await PaginatedList<Exercises>.CreateAsync(userFavoriteExerciseList, pageNumber ?? 1, 8);
                                        }
                                        break;
                                    }
                                }
                                break;
                            // If the sortBy parameter is HIIT, order the exercises by HIIT.
                            case "HIIT":
                                // Loop through the exercise types.
                                foreach (var exType in exerciseTypes)
                                {
                                    // If the exercise type is HIIT, get the exercises that are HIIT.
                                    if (exType.Name == "HIIT")
                                    {
                                        userFavoriteExerciseList = _context.UserFavoriteExercises
                                        .Where(e => e.UserId == currentUser.Id && e.Exercise.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                                        .Include(e => e.Exercise.ExerciseExerciseType)
                                        .Select(e => e.Exercise)
                                        .ToList();

                                        // If the user has favorited exercises, set the View Models FavoriteExercises to a paginated list of the exercises.
                                        if (userFavoriteExerciseList.Count() > 0)
                                        {
                                            evm.FavoriteExercises = await PaginatedList<Exercises>.CreateAsync(userFavoriteExerciseList, pageNumber ?? 1, 8);
                                        }
                                        break;
                                    }
                                }
                                break;
                            // If the sortBy parameter is Weightlifting, order the exercises by Weightlifting.
                            case "Weightlifting":
                                // Loop through the exercise types.
                                foreach (var exType in exerciseTypes)
                                {
                                    // If the exercise type is Weightlifting, get the exercises that are Weightlifting.
                                    if (exType.Name == "Weightlifting")
                                    {
                                        userFavoriteExerciseList = _context.UserFavoriteExercises
                                        .Where(e => e.UserId == currentUser.Id && e.Exercise.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                                        .Include(e => e.Exercise.ExerciseExerciseType)
                                        .Select(e => e.Exercise)
                                        .ToList();

                                        // If the user has favorited exercises, set the View Models FavoriteExercises to a paginated list of the exercises.
                                        if (userFavoriteExerciseList.Count() > 0)
                                        {
                                            evm.FavoriteExercises = await PaginatedList<Exercises>.CreateAsync(userFavoriteExerciseList, pageNumber ?? 1, 8);
                                        }
                                        break;
                                    }
                                }
                                break;
                            // If the sortBy parameter is Balance/Flexibility, order the exercises by Balance/Flexibility.
                            case "Balance/Flexibility":
                                // Loop through the exercise types.
                                foreach (var exType in exerciseTypes)
                                {
                                    // If the exercise type is Balance/Flexibility, get the exercises that are Balance/Flexibility.
                                    if (exType.Name == "Balance/Flexibility")
                                    {
                                        userFavoriteExerciseList = _context.UserFavoriteExercises
                                        .Where(e => e.UserId == currentUser.Id && e.Exercise.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                                        .Include(e => e.Exercise.ExerciseExerciseType)
                                        .Select(e => e.Exercise)
                                        .ToList();

                                        // If the user has favorited exercises, set the View Models FavoriteExercises to a paginated list of the exercises.
                                        if (userFavoriteExerciseList.Count() > 0)
                                        {
                                            evm.FavoriteExercises = await PaginatedList<Exercises>.CreateAsync(userFavoriteExerciseList, pageNumber ?? 1, 8);
                                        }
                                        break;
                                    }
                                }
                                break;
                            // If the sortBy parameter is Yoga, order the exercises by Yoga.
                            case "Yoga":
                                // Loop through the exercise types.
                                foreach (var exType in exerciseTypes)
                                {
                                    // If the exercise type is Yoga, get the exercises that are Yoga.
                                    if (exType.Name == "Yoga")
                                    {
                                        userFavoriteExerciseList = _context.UserFavoriteExercises
                                        .Where(e => e.UserId == currentUser.Id && e.Exercise.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                                        .Include(e => e.Exercise.ExerciseExerciseType)
                                        .Select(e => e.Exercise)
                                        .ToList();

                                        // If the user has favorited exercises, set the View Models FavoriteExercises to a paginated list of the exercises.
                                        if (userFavoriteExerciseList.Count() > 0)
                                        {
                                            evm.FavoriteExercises = await PaginatedList<Exercises>.CreateAsync(userFavoriteExerciseList, pageNumber ?? 1, 8);
                                        }
                                        break;
                                    }
                                }
                                break;
                        }

                        // If there is no sorting option selected, order the exercises by name ascending.
                        if (userFavoriteExerciseList.Count() > 0)
                        {
                            evm.FavoriteExercises = await PaginatedList<Exercises>.CreateAsync(userFavoriteExerciseList, pageNumber ?? 1, 8);
                        }
                    }
                    break;
                default:
                    // If the search string is not null or empty, get the exercises that contain the search string.
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        // Get the exercises that contain the search string.
                        var exercises = _context.Exercises.Where(e => e.Name.Contains(searchString));

                        // If the exercises list has exercises, set the View Models Exercises to a paginated list of the exercises.
                        if (exercises.Count() > 0)
                        {
                            evm.Exercises = await PaginatedList<Exercises>.CreateAsync(exercises, 1, 8);

                        }
                    } else
                    {
                        // Switch on the sortBy parameter.
                        switch (sortBy)
                        {
                            // If the sortBy parameter is name_desc, order the exercises by name in descending order.
                            case "name_desc":
                                var exercises = _context.Exercises.AsNoTracking().OrderByDescending(e => e.Name);

                                // If the exercises list has exercises, set the View Models Exercises to a paginated list of the exercises.
                                if (exercises.Count() > 0)
                                {
                                    evm.Exercises = await PaginatedList<Exercises>.CreateAsync(exercises, pageNumber ?? 1, 8);
                                }
                                break;
                            // If the sortBy parameter is name_asc, order the exercises by name in ascending order.
                            case "name_asc":
                                exercises = _context.Exercises.AsNoTracking().OrderBy(e => e.Name);

                                // If the exercises list has exercises, set the View Models Exercises to a paginated list of the exercises.
                                if (exercises.Count() > 0)
                                {
                                    evm.Exercises = await PaginatedList<Exercises>.CreateAsync(exercises, pageNumber ?? 1, 8);
                                }
                                break;
                            // If the sortBy parameter is Cardio, order the exercises by Cardio.
                            case "Cardio":
                                // Loop through the exercise types.
                                foreach (var exType in exerciseTypes)
                                {
                                    // If the exercise type is Cardio, get the exercises that are Cardio.
                                    if (exType.Name == "Cardio")
                                    {
                                        exercises = _context.Exercises.Include(e => e.ExerciseExerciseType)
                                        .Where(e => e.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                                        .OrderBy(e => e.Name);

                                        // If the exercises list has exercises, set the View Models Exercises to a paginated list of the exercises.
                                        if (exercises.Count() > 0)
                                        {
                                            evm.Exercises = await PaginatedList<Exercises>.CreateAsync(exercises, pageNumber ?? 1, 8);
                                        }
                                        break;
                                    }
                                }
                                break;
                            // If the sortBy parameter is HIIT, order the exercises by HIIT.
                            case "HIIT":
                                // Loop through the exercise types.
                                foreach (var exType in exerciseTypes)
                                {
                                    // If the exercise type is HIIT, get the exercises that are HIIT.
                                    if (exType.Name == "HIIT")
                                    {
                                        exercises = _context.Exercises.Include(e => e.ExerciseExerciseType)
                                        .Where(e => e.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                                        .OrderBy(e => e.Name);

                                        // If the exercises list has exercises, set the View Models Exercises to a paginated list of the exercises.
                                        if (exercises.Count() > 0)
                                        {
                                            evm.Exercises = await PaginatedList<Exercises>.CreateAsync(exercises, pageNumber ?? 1, 8);
                                        }
                                        break;
                                    }
                                }
                                break;
                            // If the sortBy parameter is Weightlifting, order the exercises by Weightlifting.
                            case "Weightlifting":
                                // Loop through the exercise types.
                                foreach (var exType in exerciseTypes)
                                {
                                    // If the exercise type is Weightlifting, get the exercises that are Weightlifting.
                                    if (exType.Name == "Weightlifting")
                                    {
                                        exercises = _context.Exercises.Include(e => e.ExerciseExerciseType)
                                        .Where(e => e.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                                        .OrderBy(e => e.Name);

                                        // If the exercises list has exercises, set the View Models Exercises to a paginated list of the exercises.
                                        if (exercises.Count() > 0)
                                        {
                                            evm.Exercises = await PaginatedList<Exercises>.CreateAsync(exercises, pageNumber ?? 1, 8);
                                        }
                                        break;
                                    }
                                }
                                break;
                            // If the sortBy parameter is Balance/Flexibility, order the exercises by Balance/Flexibility.
                            case "Balance/Flexibility":
                                // Loop through the exercise types.
                                foreach (var exType in exerciseTypes)
                                {
                                    // If the exercise type is Balance/Flexibility, get the exercises that are Balance/Flexibility.
                                    if (exType.Name == "Balance/Flexibility")
                                    {
                                        exercises = _context.Exercises.Include(e => e.ExerciseExerciseType)
                                        .Where(e => e.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                                        .OrderBy(e => e.Name);

                                        // If the exercises list has exercises, set the View Models Exercises to a paginated list of the exercises.
                                        if (exercises.Count() > 0)
                                        {
                                            evm.Exercises = await PaginatedList<Exercises>.CreateAsync(exercises, pageNumber ?? 1, 8);
                                        }
                                        break;
                                    }
                                }
                                break;
                            // If the sortBy parameter is Yoga, order the exercises by Yoga.
                            case "Yoga":
                                // Loop through the exercise types.
                                foreach (var exType in exerciseTypes)
                                {
                                    // If the exercise type is Yoga, get the exercises that are Yoga.
                                    if (exType.Name == "Yoga")
                                    {
                                        exercises = _context.Exercises.Include(e => e.ExerciseExerciseType)
                                        .Where(e => e.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                                        .OrderBy(e => e.Name);

                                        // If the exercises list has exercises, set the View Models Exercises to a paginated list of the exercises.
                                        if (exercises.Count() > 0)
                                        {
                                            evm.Exercises = await PaginatedList<Exercises>.CreateAsync(exercises, pageNumber ?? 1, 8);
                                        }
                                        break;
                                    }
                                }
                                break;
                            // If the sortBy parameter is not set, get all exercises.
                            default:
                                if (_context.Exercises.Count() > 0)
                                {
                                    evm.Exercises = await PaginatedList<Exercises>.CreateAsync(_context.Exercises.AsNoTracking(), pageNumber ?? 1, 8);
                                } else
                                {
                                    evm.Exercises = null;
                                }
                                
                                break;
                        }
                    }
                    
                    // Determine if the user has favorited the exercises.
                    _utility.determineFavoriteExercises(evm.Exercises, userFavoriteExercises);
                    break;
            }

            return View(evm);

        }

        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Exercises == null)
            {
                return NotFound();
            }

            var exercises = await _context.Exercises
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercises == null)
            {
                return NotFound();
            }

            return View(exercises);
        }

        // GET: Exercises/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exercises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Exercises exercises)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exercises);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exercises);
        }

        // GET: Exercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Exercises == null)
            {
                return NotFound();
            }

            var exercises = await _context.Exercises.FindAsync(id);
            if (exercises == null)
            {
                return NotFound();
            }
            return View(exercises);
        }

        // POST: Exercises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Exercises exercises)
        {
            if (id != exercises.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exercises);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExercisesExists(exercises.Id))
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
            return View(exercises);
        }

        // GET: Exercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Exercises == null)
            {
                return NotFound();
            }

            var exercises = await _context.Exercises
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercises == null)
            {
                return NotFound();
            }

            return View(exercises);
        }

        // POST: Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Exercises == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Exercises'  is null.");
            }
            var exercises = await _context.Exercises.FindAsync(id);
            if (exercises != null)
            {
                _context.Exercises.Remove(exercises);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExercisesExists(int id)
        {
          return _context.Exercises.Any(e => e.Id == id);
        }
    }
}
