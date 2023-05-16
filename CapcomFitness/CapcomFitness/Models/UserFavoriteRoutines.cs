using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapcomFitness.Models
{
    public class UserFavoriteRoutines
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key
        [ForeignKey("Routine")]
        public int RoutineId { get; set; }

        // Foreign Key
        [ForeignKey("User")]
        public string UserId { get; set; }

        // Reference Navigation Property
        public virtual Routines? Routine { get; set; }

        // Reference Navigation Property
        public virtual AppUser? User { get; set; }
    }
}
