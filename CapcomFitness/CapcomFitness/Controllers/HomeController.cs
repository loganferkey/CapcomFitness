using CapcomFitness.Data;
using CapcomFitness.Models;
using CapcomFitness.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace CapcomFitness.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            Random rng = new Random();
            List<Exercises> exercises = await _context.Exercises.Include(e => e.ExerciseExerciseType).ThenInclude(eet => eet.ExerciseType).ToListAsync();
            var randExercises = exercises.OrderBy(x => rng.Next()).ToList().Take(3);

            // obtain a dictionary with the exercise id values in randExercises and their corresponding exercise type id values
            Dictionary<int, List<int>> exTypes = await GetExerciseTypes(randExercises);

            // loop through these exercises and the dictionary from the last step to bind the exercise type values to the exercises
            foreach (var exercise in randExercises)
            {
                if (exTypes.ContainsKey(exercise.Id))
                {
                    exercise.ExType = exTypes[exercise.Id].Select(id => id.ToString()).ToList();
                }
            }

            return View(randExercises);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<Dictionary<int, List<int>>> GetExerciseTypes(IEnumerable<Exercises> exercises)
        {
            // create a new dictionary to store the exercises with their corresponding exercise types
            Dictionary<int, List<int>> exTypes = new Dictionary<int, List<int>>();

            // if the list of exercises exists...
            if (exercises != null)
            {
                // loop through the exercises, get their exercise type values from the EET table, and add them to the dictionary
                foreach (var exercise in exercises)
                {
                    int exId = exercise.Id;
                    var exTypeIds = await _context.ExerciseExerciseType
                        .Where(e => e.ExerciseId == exId)
                        .Select(e => e.ExerciseTypeId)
                        .ToListAsync();

                    exTypes.Add(exId, exTypeIds);
                }
            }

            return exTypes;
        }

        public async Task<IActionResult> GenerateRoutine(string exerciseType)
        {
            Random rng = new Random();
            List<Exercises> allExercises = await _context.Exercises.Include(e => e.ExerciseExerciseType).ToListAsync();
            List<ExerciseTypes> exerciseTypes = await _context.ExerciseTypes.ToListAsync();
            var randExercises = new List<Exercises>();

            switch (exerciseType)
            {
                case "Cardio":
                    foreach (var exType in exerciseTypes)
                    {
                        if (exType.Name == "Cardio")
                        {
                            randExercises = allExercises
                            .Where(e => e.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id))
                            .OrderBy(x => rng.Next()).Take(3).ToList();
                        }
                    }
                    break;
                case "HIIT":
                    foreach (var exType in exerciseTypes)
                    {
                        if (exType.Name == "HIIT")
                        {
                            randExercises = allExercises.Where(e => e.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id)).OrderBy(x => rng.Next()).Take(3).ToList();
                        }
                    }
                    break;
                case "Weightlifting":
                    foreach (var exType in exerciseTypes)
                    {
                        if (exType.Name == "Weightlifting")
                        {
                            randExercises = allExercises.Where(e => e.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id)).OrderBy(x => rng.Next()).Take(3).ToList();
                        }
                    }
                    break;
                case "Balance/Flexibility":
                    foreach (var exType in exerciseTypes)
                    {
                        if (exType.Name == "Balance/Flexibility")
                        {
                            randExercises = allExercises.Where(e => e.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id)).OrderBy(x => rng.Next()).Take(3).ToList();
                        }
                    }
                    break;
                case "Yoga": 
                    foreach (var exType in exerciseTypes)
                    {
                        if (exType.Name == "Yoga")
                        {
                            randExercises = allExercises.Where(e => e.ExerciseExerciseType.Any(eet => eet.ExerciseTypeId == exType.Id)).OrderBy(x => rng.Next()).Take(3).ToList();
                        }
                    }
                    break;
                default:
                    randExercises = allExercises.OrderBy(x => rng.Next()).Take(3).ToList();
                    break;
            }

            return PartialView("_RoutinePopup", randExercises);
        }

    }
}