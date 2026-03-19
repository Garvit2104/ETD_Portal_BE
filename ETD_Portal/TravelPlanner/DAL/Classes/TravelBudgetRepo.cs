using ETD_Portal.Data;
using ETD_Portal.Models;
using ETD_Portal.TravelPlanner.DAL.Interfaces;

namespace ETD_Portal.TravelPlanner.DAL.Classes
{
    public class TravelBudgetRepo : ITravelBudgetRepo
    {
        private readonly ETDPortalDbContext _context;

        public TravelBudgetRepo(ETDPortalDbContext context)
        {
            this._context = context;
        }

        public async Task AddBudgetAllocation(TravelBudgetAllocation travelBudgetAllocation)
        {
            await _context.TravelBudgetAllocations.AddAsync(travelBudgetAllocation);
            await _context.SaveChangesAsync();
        }
    }
}
