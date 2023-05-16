using System.ComponentModel.DataAnnotations;

namespace CapcomFitness.Models
{
    public class ExerciseTypes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Reference Collection Property
        public virtual ICollection<ExerciseExerciseType>? ExerciseExerciseTypes { get; set; }
    }
}
