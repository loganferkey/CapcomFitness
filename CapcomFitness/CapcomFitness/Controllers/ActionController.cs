using CapcomFitness.Data;
using CapcomFitness.Models;
using CapcomFitness.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

namespace CapcomFitness.Controllers
{
    // The main purpose of this controller is to run actions that don't have their own respective page, like a user favoriting an item then the page refreshing directly after
    // Admin actions probably should go in the admin controller not here, see AdminController
    [Authorize]
    public class ActionController : Controller
    {
        private readonly IUtility _utility;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ActionController(ApplicationDbContext context, IUtility utility, UserManager<AppUser> userManager) 
        {
            _utility = utility;
            _context = context;
            _userManager = userManager;
        }

        // Allows the user to favorite their exercises.
        public async Task<IActionResult> FavoriteExercise(int? id, string? Tab, int? PageNumber)
        {
            // Gets the current user.
            var user = await _utility.getHTTPContextUser();
            // Gets the passed in exercise.
            var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id);

            // If the exercise is not null, Creates a new favorite exercise, adds it to the database and saves changes.
            if (exercise != null && user != null)
            {
                UserFavoriteExercises favoriteExercises = new UserFavoriteExercises
                {
                    ExerciseId = exercise.Id,
                    UserId = user.Id
                };

                _context.UserFavoriteExercises.Add(favoriteExercises);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("UserExercises", "exercises", new { tab = Tab, pageNumber = PageNumber });
        }

        // Allows the user to Unfavorite their favorite exercises.
        public async Task<IActionResult> UnfavoriteExercise(int? id, string? Tab, int? PageNumber)
        {
            // Gets the current user.
            var user = await _utility.getHTTPContextUser();
            // Gets the passed in exercise.
            var exercise = await _context.UserFavoriteExercises.Where(e => e.ExerciseId == id && e.UserId == user.Id).FirstOrDefaultAsync();

            // If exercise is not null, remove it from the favorite exercise table, and save changes.
            if (exercise != null)
            {
                _context.UserFavoriteExercises.Remove(exercise);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("UserExercises", "exercises", new { tab = Tab, pageNumber = PageNumber });
        }

        // Allows the user to favorite their exercises.
        public async Task<IActionResult> FavoriteRoutine(int? id, string? tab, int? PageNumber)
        {
            // Gets the current user.
            var user = await _utility.getHTTPContextUser();
            // Gets the passed in exercise.
            var routine = await _context.Routines.FirstOrDefaultAsync(e => e.Id == id);

            // If the routine is not null, Creates a new favorite routine, adds it to the database and saves changes.
            if (routine != null && user != null)
            {
                UserFavoriteRoutines favoriteRoutine = new UserFavoriteRoutines
                {
                    RoutineId = routine.Id,
                    UserId = user.Id
                };

                _context.UserFavoriteRoutines.Add(favoriteRoutine);
                await _context.SaveChangesAsync();
            }

            // Takes them back to exactly where they were! 
            return RedirectToAction("UserRoutines", "Routines", new { Tab = tab, pageNumber = PageNumber });
        }

        // Allows the user to Unfavorite their favorite exercises.
        public async Task<IActionResult> UnfavoriteRoutine(int? id, string? tab, int? PageNumber)
        {
            // Gets the current user.
            var user = await _utility.getHTTPContextUser();
            // Gets the passed in exercise.
            var routine = await _context.UserFavoriteRoutines.Where(e => e.RoutineId == id && e.UserId == user.Id).FirstOrDefaultAsync();

            // If exercise is not null, remove it from the favorite exercise table, and save changes.
            if (routine != null)
            {
                _context.UserFavoriteRoutines.Remove(routine);
                await _context.SaveChangesAsync();
            }

            // Takes them back to exactly where they were!
            return RedirectToAction("UserRoutines", "Routines", new { tab = tab, pageNumber = PageNumber });
        }

        [Authorize]
        public async Task<IActionResult> AddWeightHistory(float? weight)
        {   
			var user = await _utility.getHTTPContextUser();
            if (weight != null)
            {
                WeightHistory history = new WeightHistory { DateEntered = DateTime.Now, UserId = user!.Id, Weight = (float)Math.Clamp((double)weight,0,1000) };
                _context.WeightHistory.Add(history);
                await _context.SaveChangesAsync();
            }
			return RedirectToAction("Tracker", "WeightHistories");

        }

        public async Task<IActionResult> AddRoutineExercise([Bind("Sets,Reps,AdditionalInfo,RoutineId,ExerciseId")] RoutineExercises routineExercise, string routineName)
        {
            if (ModelState.IsValid)
            {
                // If the form is valid, i.e all fields filled out, add it to the database and save changes
                _context.RoutineExercises.Add(routineExercise);
                await _context.SaveChangesAsync();
            }

            // Then redirect back to the same page it was at
            return RedirectToAction("UserRoutinesDetail", "Routines", new { id = routineName });
        }

        public async Task<IActionResult> RemoveRoutineExercise(int? id, string routineName)
        {
            if (ModelState.IsValid)
            {
                RoutineExercises routineExercise = _context.RoutineExercises.Where(re => re.Id == id).FirstOrDefault();
                if (routineExercise != null) 
                {
                    _context.RoutineExercises.Remove(routineExercise);
                    await _context.SaveChangesAsync();
                }
            }

            // Then redirect back to the same page it was at
            return RedirectToAction("UserRoutinesDetail", "Routines", new { id = routineName });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string UserId, string CommenterId, string Comment)
        {
            var newComment = new Comments
            {
                UserId = UserId,
                CommenterId = CommenterId,
                Comment = Comment,
                DateCreated = DateTime.Now
            };

            var commenter = await _userManager.FindByIdAsync(CommenterId);

            if (newComment.Comment != "" && newComment.Comment != null)
            {
                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();
                var response = new
                {
                    success = true,
                    nick = commenter.Nickname,
                    date = newComment.DateCreated.ToString(),
                    commentId = newComment.Id
                };
                return Json(response);
            } else
            {
                return Json(new { error = true });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveComment(int commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment != null)
            {
                // Found it!
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return Json(new {success = true });
            }
            else
            {
                return Json(new { error = true });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int? limit, string profileId)
        {
            List<Comments> reqComments = await _context.Comments.Where(c => c.UserId == profileId).Include(c => c.Commenter).OrderByDescending(c => c.DateCreated).ToListAsync();
            List<string> htmlComments = new List<string>();
            AppUser? user = await _utility.getHTTPContextUser();

            if (reqComments.Any())
            {
                int listLimit = limit != null ? Math.Clamp((int)limit, 0, reqComments.Count) : reqComments.Count;
                for (int i = 0; i < listLimit; i++)
                {
                    var comment = reqComments[i];
                    string profileImg = (comment.Commenter.ProfileImageUrl == "" || comment.Commenter.ProfileImageUrl == null) ? "/images/user.png" : comment.Commenter.ProfileImageUrl;
                    string htmlFormatted = $@"
                        <div data-comment-id=""{comment.Id}"" class=""comment d-flex flex-column gap-2"">
                            <div class=""d-flex flex-row text-white align-items-center justify-content-between"">
                                <div class=""d-flex flex-row gap-2 text-white align-items-center"">
                                    <img src=""{profileImg}"" width=""26px"" height=""26px"" style=""border-radius: 9999px;"" />
                                    <p class=""p-0 m-0"">{comment.Commenter!.Nickname}</p>
                                </div>
                                <div class=""d-flex flex-row gap-2"">
                                    <p class=""p-0 m-0 text-white-50"">{comment.DateCreated}</p>
                                    {(user != null && comment.Commenter.Id == user.Id ? $@"<a data-comment-id=""{comment.Id}"" class=""DeleteComment"" href=""#comments""><i class=""fa-solid fa-trash-can text-white""></i></a>" : "")}
                                </div>
                            </div>
                            <p class=""text-white p-0 m-0"">{comment.Comment}</p>
                        </div>";
                    htmlComments.Add(htmlFormatted);
                }
                return Json(new
                {
                    success = true,
                    comments = htmlComments
                });
            }

            // No comments pulled
            return Json(new
            {
                success = true,
                comments = ""
            });
        }
    }
}
