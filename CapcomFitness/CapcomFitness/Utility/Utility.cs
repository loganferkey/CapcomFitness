using CapcomFitness.Models;
using Microsoft.AspNetCore.Identity;

namespace CapcomFitness.Utility
{
    /// <summary>
    /// Service class for the website, will hold a bunch of different useful functions, you can inject it in the controller or in views directly to get the current user
    /// </summary>
    public class Utility : IUtility
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public Utility(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._userManager = userManager;
        }

        public async Task<AppUser?> getHTTPContextUser()
        {
            AppUser? user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user != null)
            {
                if (user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
                {
                    // When people don't have an image setup make sure it's the default
                    user.ProfileImageUrl = "/images/user.png";
                }
            }

            return user;
        }

        /// <summary>
        /// Modifies the UserFavorited property on each routine if the user has them in their favorites, for use on the view
        /// </summary>
        /// <param name="passedRoutines">A list of all routines to be displayed in list</param>
        /// <param name="favoriteRoutines">A list of favorite routines only the current user has</param>
        /// <returns>Returns a modified list of routines with correct UserFavorited tag</returns>
        public IEnumerable<Routines>? determineFavoriteRoutines(PaginatedList<Routines>? passedRoutines = null, IEnumerable<UserFavoriteRoutines>? favoriteRoutines = null)
        {
            if (passedRoutines == null)
                return null;

            foreach (Routines r in passedRoutines)
            {
                // Escape case if the user doesn't have any favorite routines
                if (favoriteRoutines == null)
                    break;

                // Unfortunately this is a double loop :P
                foreach (UserFavoriteRoutines ufr in favoriteRoutines)
                {
                    // If the routine matches the favorite routine, set the property to true and break, so it doesn't loop through everything
                    if (ufr.Routine.Id == r.Id)
                        r.UserFavorited = true;
                    continue;
                }
            }

            return passedRoutines;
        }

		/// <summary>
		/// Modifies the UserFavorited property on each exercise if the user has them in their favorites, for use on the view
		/// </summary>
		/// <param name="passedRoutines">A list of all routines to be displayed in list</param>
		/// <param name="favoriteRoutines">A list of favorite routines only the current user has</param>
		/// <returns>Returns a modified list of routines with correct UserFavorited tag</returns>
		public IEnumerable<Exercises>? determineFavoriteExercises(PaginatedList<Exercises>? passedExercises = null, IEnumerable<UserFavoriteExercises>? favoriteExercises = null)
		{
			if (passedExercises == null)
				return null;

			foreach (Exercises e in passedExercises)
			{
				// Escape case if the user doesn't have any favorite routines
				if (favoriteExercises == null)
					break;

				// Unfortunately this is a double loop :P
				foreach (UserFavoriteExercises ufe in favoriteExercises)
				{
					// If the routine matches the favorite routine, set the property to true and break, so it doesn't loop through everything
					if (ufe.Exercise.Id == e.Id)
						e.UserFavorited = true;
					continue;
				}
			}

			return passedExercises;
		}
	}
}
