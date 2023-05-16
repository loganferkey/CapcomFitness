using CapcomFitness.Data;
using CapcomFitness.Models;
using CapcomFitness.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.Common;
using System.Text.Json;

namespace CapcomFitness.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SeedUsersAndRoles()
        {
            Feedback feedback = new Feedback();
            ToastNotification toast = new ToastNotification("");

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                feedback.SuccessMessages.Add("{admin} role created successfully");
            }
            else { feedback.ErrorMessages.Add("{admin} role already exists"); }

            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                feedback.SuccessMessages.Add("{user} role created successfully");
            }
            else { feedback.ErrorMessages.Add("{user} role already exists"); }

            if (await _userManager.FindByEmailAsync("admin@flex.com") == null)
            {
                var newAdminUser = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@flex.com",
                    EmailConfirmed = true,
                    Age = 21,
                    Nickname = "admin",
                    Joined = DateTime.Now
                };
                var result = await _userManager.CreateAsync(newAdminUser, "admin");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                    feedback.SuccessMessages.Add("{admin} account created successfully");
                }
                else
                {
                    feedback.ErrorMessages.Add($"Failed to create {newAdminUser.UserName} account");
                }
            }
            else { feedback.ErrorMessages.Add("{admin} account already exists"); }

            if (await _userManager.FindByEmailAsync("user@flex.com") == null)
            {
                var newUser = new AppUser()
                {
                    UserName = "user",
                    Email = "user@flex.com",
                    EmailConfirmed = true,
                    Age = 22,
                    Nickname = "user",
                    Joined = DateTime.Now
                };
                var result = await _userManager.CreateAsync(newUser, "user");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                    feedback.SuccessMessages.Add("{user} account created successfully");
                }
                else
                {
                    feedback.ErrorMessages.Add($"Failed to create {newUser.UserName} account");
                }
            }
            else { feedback.ErrorMessages.Add("{user} account already exists"); }

            ToastNotification.AddFeedbackToTempData(feedback, TempData);
            TempData["Toast"] = toast.ToString();

            return View("Index");
        }


        public async Task<IActionResult> SeedExerciseTypes()
        {
            // create new feedback and variables to store duplicate and success quantities
            Feedback feedback = new Feedback();
            int duplicate = 0;
            int success = 0;

            // read JSON data and deserialize it to a list of ExerciseTypes
            string exerciseTypeJsondata = System.IO.File.ReadAllText("Data/ExerciseTypes.json");
            var exerciseTypes = System.Text.Json.JsonSerializer.Deserialize<List<ExerciseTypes>>(exerciseTypeJsondata);

            // retrieve all existing ExerciseTypes from the database
            List<ExerciseTypes> allExerciseTypes = await _context.ExerciseTypes.ToListAsync();

            // if exerciseTypes list has data
            if (exerciseTypes != null)
            {
                // check each item in list for a match in the database list
                foreach (var exerciseType in exerciseTypes)
                {
                    var skip = false;
                    foreach (ExerciseTypes et in allExerciseTypes)
                    {
                        if (et.Name.ToLower().Trim() == exerciseType.Name.ToLower().Trim())
                        {
                            // skip matches and add the duplicate for feedback
                            skip = true;
                            duplicate += 1;
                            break;
                        }
                    }

                    // if no matches were found and skipped, and the ExerciseType to the database and count the successful add for feedback
                    if (skip == false)
                    {
                        System.Diagnostics.Debug.WriteLine($"{exerciseType.Name} {exerciseType.Description}");

                        _context.ExerciseTypes.Add(exerciseType);
                        await _context.SaveChangesAsync();
                        success += 1;
                    }
                }
            }

            //create toast messages for errors and successes, if they exist
            if (success > 0)
            {
                ToastNotification successToast = new ToastNotification(success + " exercise types added successfully");
                TempData["SuccessMessage"] = successToast.ToString();
            }

            if (duplicate > 0)
            {
                ToastNotification errorToast = new ToastNotification(duplicate + " duplicate exercise types skipped");
                TempData["ErrorMessage"] = errorToast.ToString();
            }

            //add feedback to TempData for Index view
            ToastNotification.AddFeedbackToTempData(feedback, TempData);
            return View("Index");
        }


        public async Task<IActionResult> SeedExercises()
        {
            // create new feedback and variables to store duplicate and success quantities
            Feedback feedback = new Feedback();
            int duplicate = 0;
            int success = 0;

            // read JSON data and deserialize it to a list of Exercises
            string exerciseJsondata = System.IO.File.ReadAllText("Data/Exercises.json");
            var exercises = System.Text.Json.JsonSerializer.Deserialize<List<Exercises>>(exerciseJsondata);

            // retrieve all existing Exercises from the database
            List<Exercises> allExercises = await _context.Exercises.ToListAsync();

            // if exercises list has data
            if (exercises != null)
            {
                // check each item in list for a match in the database list
                foreach (var exercise in exercises)
                {
                    var skip = false;
                    foreach (Exercises e in allExercises)
                    {
                        if (e.Name.ToLower().Trim() == exercise.Name.ToLower().Trim())
                        {
                            // skip matches and add the duplicate for feedback
                            skip = true;
                            duplicate += 1;
                            break;
                        }
                    }

                    // if not matchs were found and skipped, add the Exercise to the database and count the successful add for feedback
                    if (skip == false)
                    {
                        System.Diagnostics.Debug.WriteLine($"{exercise.Name} {exercise.Description}");

                        _context.Exercises.Add(exercise);
                        await _context.SaveChangesAsync();
                        success += 1;
                    }
                }
            }

            // create toast notification messages for any successfull adds or duplicates found
            if (success > 0)
            {
                ToastNotification successToast = new ToastNotification(success + " exercises added successfully");
                TempData["SuccessMessage"] = successToast.ToString();
            }

            if (duplicate > 0)
            {
                ToastNotification errorToast = new ToastNotification(duplicate + " duplicate exercises skipped");
                TempData["ErrorMessage"] = errorToast.ToString();
            }

            // add feedback to TempData for Index view
            ToastNotification.AddFeedbackToTempData(feedback, TempData);

            return View("Index");
        }

        public async Task<IActionResult> SeedEET()
        {
            // define feedback variables 
            Feedback feedback = new Feedback();
            int duplicate = 0;
            int success = 0;



            // read and store json data 
            string exTypeData = System.IO.File.ReadAllText("Data/Exercises.json");
            var exercises = System.Text.Json.JsonSerializer.Deserialize<List<Exercises>>(exTypeData);



            // create local variable for exercise exercise type record
            ExerciseExerciseType eet = new ExerciseExerciseType();



            // if there are stored exercises...
            if (exercises != null)
            {
                // loop through each exercise record
                foreach (var exercise in exercises)
                {
                    // temporarily store each ExType for the exercise
                    List<string> listTypes = exercise.ExType;



                    // gets and stores the exercise's Id value
                    int exerciseId = GetExerciseId(exercise.Name);



                    // if there is a list of exercise types...
                    if (listTypes != null)
                    {
                        // loop through each exercise type 
                        foreach (string exType in listTypes)
                        {
                            // gets and stores the exercise type's Id value
                            int exerciseTypeId = GetTypeId(exType);



                            // create new exercise/exercise type linkage for each type listed
                            eet = new ExerciseExerciseType { ExerciseId = exerciseId, ExerciseTypeId = exerciseTypeId };



                            // check if this linkage already exists in the database
                            bool eetExists = await _context.ExerciseExerciseType
                                .AnyAsync(x => x.ExerciseId == exerciseId && x.ExerciseTypeId == exerciseTypeId);



                            if (!eetExists)
                            {
                                // write the eet linkage to the EET table
                                _context.ExerciseExerciseType.Add(eet);
                                success++;
                            }
                            else
                            {
                                duplicate++;
                            }
                        }
                    }
                }



                // save changes made to table
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["Feedback"] = "No exercise/exercise type linkages to seed.";
            }



            if (success > 0)
            {
                ToastNotification successToast = new ToastNotification(success + " exercise/exercise type linkages added successfully.");
                TempData["SuccessMessage"] = successToast.ToString();
            }



            if (duplicate > 0)
            {
                ToastNotification errorToast = new ToastNotification(duplicate + " duplicate linkages skipped.");
                TempData["ErrorMessage"] = errorToast.ToString();
            }



            ToastNotification.AddFeedbackToTempData(feedback, TempData);



            return View("Index");
        }

        private int GetTypeId(string exerciseType)
        {
            // create variables to store record and Id values
            int typeId = 0;
            var result = _context.ExerciseTypes.FirstOrDefault(e => e.Name.Contains(exerciseType));
            
            // if the record exists, set its Id
            if (result != null)
                typeId = result.Id;

            return typeId;
        }

        private int GetExerciseId(string exName)
        {
            // create variables to store record and Id values
            int exerciseId = 0;
            var result = _context.Exercises.FirstOrDefault(e => e.Name.Contains(exName));

            // if the record exists, set its Id
            if (result != null)
                exerciseId = result.Id;

            return exerciseId;
        }
    }
}
