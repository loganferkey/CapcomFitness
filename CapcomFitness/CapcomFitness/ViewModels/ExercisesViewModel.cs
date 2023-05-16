using CapcomFitness.Models;
using CapcomFitness.Utility;

namespace CapcomFitness.ViewModels
{
    public class ExercisesViewModel
    {
        public AppUser? User { get; set; }

        public PaginatedList<Exercises> Exercises { get; set; }

        public PaginatedList<Exercises> FavoriteExercises { get; set; }

        public IEnumerable<ExerciseTypes> ExerciseTypes { get; set; }

		public string? Tab { get; set; }
    }
}
