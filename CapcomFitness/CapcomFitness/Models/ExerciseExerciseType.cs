using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapcomFitness.Models
{
    public class ExerciseExerciseType
    {
        // Primary Key
        [Key]
        public int Id { get; set; }

        // Foreign Key
        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }

        // Foreign Key
        [ForeignKey("ExerciseType")]
        public int ExerciseTypeId { get; set; }

        // Reference Navigation Property
        public virtual Exercises? Exercise { get; set; }

        // Reference Navigation Property
        public virtual ExerciseTypes? ExerciseType { get; set; }
    }
}
