using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapcomFitness.Models
{
    public class RoutineExercises
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Sets { get; set; }

        [Required]
        public int Reps { get; set; }

        public string? AdditionalInfo { get; set; }

        // Foreign Key
        [ForeignKey("Routine")]
        public int RoutineId { get; set; }

        // Foreign Key
        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }

        // Reference Navigation Property
        public virtual Routines? Routine { get; set; }

        // Reference Navigation Property
        public virtual Exercises? Exercise { get; set; }
        
    }
}
