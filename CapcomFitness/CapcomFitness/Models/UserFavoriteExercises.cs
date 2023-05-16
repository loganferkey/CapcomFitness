using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapcomFitness.Models
{
    public class UserFavoriteExercises
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key
        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }

        // Foreign Key
        [ForeignKey("User")]
        public string UserId { get; set; }

        // Reference Navigation Property
        public virtual Exercises? Exercise { get; set; }

        // Reference Navigation Property
        public virtual AppUser? User { get; set; }
    }
}
