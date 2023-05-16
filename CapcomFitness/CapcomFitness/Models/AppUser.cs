using CapcomFitness.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CapcomFitness.Models
{
    public class AppUser : IdentityUser
    {
        /// <summary>
        /// Field the user can display their age at
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// User's nickname that displays instead of username/changeable
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// Where the user lives/is based out of, displayed on profile
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// A link to any website the user pleases on their public profile
        /// </summary>
        public string? Website { get; set; }

        /// <summary>
        /// Biography for user profile
        /// </summary>
        public string? Biography { get; set; }

        /// <summary>
        /// The user's profile image url
        /// </summary>
        [DefaultValue("/images/user.png")]
        public string? ProfileImageUrl { get; set; }

        /// <summary>
        /// The time the user profile was created
        /// </summary>
        public DateTime? Joined { get; set; }


        // Reference Collection Property
        public virtual ICollection<WeightHistory>? WeightHistories { get; set; }

        // Reference Collection Property
        public virtual ICollection<UserFavoriteExercises>? FavoriteExercises { get; set; }

        // Reference Collection Property
        public virtual ICollection<UserFavoriteRoutines>? FavoriteRoutines { get; set; }

        public virtual ICollection<Routines>? CreatedRoutines { get; set; }
    }
}
