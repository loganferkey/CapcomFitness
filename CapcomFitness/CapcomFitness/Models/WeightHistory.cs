using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapcomFitness.Models
{
    public class WeightHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public float Weight { get; set; }


        public DateTime DateEntered { get; set; }

        // Foreign Key
        [ForeignKey("User")]
        public string UserId { get; set; }

        // Reference Navigation Property
        public virtual AppUser? User { get; set; }
    }
}
