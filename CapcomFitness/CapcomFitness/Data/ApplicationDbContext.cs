using CapcomFitness.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CapcomFitness.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Provides database sets for each table to be used to query or save instances.
        /// </summary>
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<UserFavoriteExercises> UserFavoriteExercises { get; set; }
        public DbSet<UserFavoriteRoutines> UserFavoriteRoutines { get; set; }
        public DbSet<WeightHistory> WeightHistory { get; set; }
        public DbSet<Exercises> Exercises { get; set; }
        public DbSet<Routines> Routines { get; set; }
        public DbSet<RoutineExercises> RoutineExercises { get; set; }
        public DbSet<ExerciseTypes> ExerciseTypes { get; set; }
        public DbSet<ExerciseExerciseType> ExerciseExerciseType { get; set; }
        public DbSet<Comments> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // I remember this was necessary for JSON for some reason :/
            base.OnModelCreating(builder);
        }
    }
}
