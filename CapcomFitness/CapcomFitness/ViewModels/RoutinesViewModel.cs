using CapcomFitness.Models;
using CapcomFitness.Utility;

namespace CapcomFitness.ViewModels
{
    public class RoutinesViewModel
    {
        public PaginatedList<Routines> Routines { get; set; }

        public PaginatedList<Routines> FavoriteRoutines { get; set; }
        public string? Tab { get; set; }
        public AppUser? User { get; set; }

    }
}
