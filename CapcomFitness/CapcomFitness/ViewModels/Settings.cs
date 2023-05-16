using CapcomFitness.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CapcomFitness.ViewModels
{
    public class Settings
    {
        [Display(Name = "Age")]
        [Range(13, 150, ErrorMessage = "You can't be under 13 or over 150!")]
        public int? Age { get; set; }

        [Display(Name = "Nickname")]
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 0)]
        public string? Nickname { get; set; }

        [Display(Name = "Location")]
        [StringLength(60, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 0)]
        public string? Location { get; set; }

        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Display(Name = "Bio")]
        [StringLength(160, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 0)]
        public string? Biography { get; set; }

        public IFormFile? Image { get; set; }
    }
}
