using CapcomFitness.Models;

namespace CapcomFitness.ViewModels
{
    public class OverviewViewModel
    {
        public AppUser? Profile { get; set; }
        public AppUser? User { get; set;}
        public IEnumerable<Comments>? Comments { get; set; }
        public IEnumerable<Routines>? Routines { get; set; }    
    }
}
