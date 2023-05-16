using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapcomFitness.Models
{
    public class Comments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Foreign Key
        [ForeignKey("User")]
        public string? UserId { get; set; }

        // Reference Navigation Property
        public virtual AppUser? User { get; set; }

        // Foreign Key
        [ForeignKey("Commenter")]
        public string? CommenterId { get; set; }

        // Reference Navigation Property
        public virtual AppUser? Commenter { get; set; }
    }
}
