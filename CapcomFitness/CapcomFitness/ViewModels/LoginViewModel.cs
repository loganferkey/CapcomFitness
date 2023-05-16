using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CapcomFitness.ViewModels
{
    public class LoginViewModel
    {
        public string? ReturnUrl { get; set; }

        public bool RememberMe { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
