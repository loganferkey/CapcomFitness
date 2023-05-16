using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapcomFitness.Models
{
    public class Exercises
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [NotMapped]
        public List<string>? ExType { get; set; }

		// This saves time in view from looping through each list two fold and we can just assign this value in controller when generating list
		[NotMapped]
		public bool? UserFavorited { get; set; } = null;

        // Reference Collection Property
        public virtual ICollection<UserFavoriteExercises>? UserFavoriteExercises { get; set; }

        // Reference Collection Property
        public virtual ICollection<RoutineExercises>? RoutineExercises { get; set; }

        // Reference Collection Property
        public virtual ICollection<ExerciseExerciseType>? ExerciseExerciseType { get; set;}
    }
}
