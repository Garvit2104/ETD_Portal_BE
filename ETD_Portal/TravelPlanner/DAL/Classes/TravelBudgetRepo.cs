using ETD_Portal.Data;
using ETD_Portal.Models;
using ETD_Portal.TravelPlanner.DAL.Interfaces;

namespace ETD_Portal.TravelPlanner.DAL.Classes
{
    public class TravelBudgetRepo : ITravelBudgetRepo
    {
        private readonly ETDPortalDbContext _context;
        private readonly ILogger<TravelBudgetRepo> _logger;

        public TravelBudgetRepo(ETDPortalDbContext context, ILogger<TravelBudgetRepo> logger)
        {
            this._context = context;
            _logger = logger;
        }

        public async Task AddBudgetAllocation(TravelBudgetAllocation travelBudgetAllocation)
        {
            try
            {
                await _context.TravelBudgetAllocations.AddAsync(travelBudgetAllocation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in AddBudgetAllocation for {TravelRequestId}", travelBudgetAllocation.TravelRequestId);
                throw;
            }
        }
    }
}
