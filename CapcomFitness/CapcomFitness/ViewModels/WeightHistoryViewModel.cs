using CapcomFitness.Models;
using CapcomFitness.Utility;

namespace CapcomFitness.ViewModels
{
    public class WeightHistoryViewModel
    {
        public AppUser? User { get; set; }

        public PaginatedList<WeightHistory> WeightHistoriesPaginated { get; set; }

        public IEnumerable<WeightHistory> WeightHistories { get; set; }
    }
}
