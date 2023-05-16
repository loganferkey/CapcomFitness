using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapcomFitness.Models
{
    public class Routines
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public bool? Private { get; set; }

        // This saves time in view from looping through each list two fold and we can just assign this value in controller when generating list
        [NotMapped]
        public bool? UserFavorited { get; set; } = null;

        // Foreign Key
        [ForeignKey("User")]
        public string? UserId { get; set; }

        // Reference Navigation Property
        public virtual AppUser? User { get; set; }

        // Reference Collection Property
        public virtual ICollection<UserFavoriteRoutines>? UserFavoriteRoutines { get; set; }

        // Reference Collection Property
        public virtual ICollection<RoutineExercises>? RoutineExercises { get; set; }
    }
}
