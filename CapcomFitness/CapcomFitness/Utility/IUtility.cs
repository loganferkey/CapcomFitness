using CapcomFitness.Models;

namespace CapcomFitness.Utility
{
    public interface IUtility
    {
        Task<AppUser?> getHTTPContextUser();
        IEnumerable<Routines>? determineFavoriteRoutines(PaginatedList<Routines>? passedRoutines = null, IEnumerable<UserFavoriteRoutines>? favoriteRoutines = null);
        IEnumerable<Exercises>? determineFavoriteExercises(PaginatedList<Exercises>? passedExercises = null, IEnumerable<UserFavoriteExercises>? favoriteExercises = null);
    }
}
