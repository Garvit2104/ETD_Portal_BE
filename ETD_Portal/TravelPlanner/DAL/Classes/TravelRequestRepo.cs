using ETD_Portal.Data;
using ETD_Portal.Models;
using ETD_Portal.TravelPlanner.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.TravelPlanner.DAL.Classes
{
    public class TravelRequestRepo : ITravelRequestRepo
    {
        private readonly ETDPortalDbContext _context;
        private readonly ILogger<TravelRequestRepo> _logger;

        public TravelRequestRepo(ETDPortalDbContext context, ILogger<TravelRequestRepo> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        public async Task<TravelRequest> CreateTravelRequest(TravelRequest travelRequest)
        {
            try
            {
                var result = await _context.TravelRequests.AddAsync(travelRequest);
                await _context.SaveChangesAsync();
                return await _context.TravelRequests
                                     .Include(t => t.Location)
                                     .FirstAsync(t => t.RequestId == travelRequest.RequestId);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a travel request.");
                throw;
            }
        }

        public async Task<IEnumerable<TravelRequest>> GetAllPendingRequests(int hrId)
        {
            try
            {
                return await _context.TravelRequests
                                     .Include(tr => tr.Location)
                                     .Where(tr => tr.ToBeApprovedByHrId == hrId
                                               && tr.RequestStatus == "New")
                                     .AsNoTracking()
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in GetAllPendingRequests for {HRId}", hrId);
                throw;
            }
        }

        public async Task<TravelRequest?> GetTravelRequestById(int? trid)
        {
            try
            {
                return await _context.TravelRequests
                                     .Include(tr => tr.Location)
                                     .FirstOrDefaultAsync(tr => tr.RequestId == trid);
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in GetTravelRequestById for {RequestId}", trid);
                throw;
            }
      
        }

        public async Task<TravelRequest?> GetTravelRequestDetailsById(int trid)
        {
            try
            {
                return await _context.TravelRequests
                                     .Include(tr => tr.Location)
                                     .Include(tr => tr.TravelBudgetAllocations)
                                     .FirstOrDefaultAsync(tr => tr.RequestId == trid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in GetTravelRequestDetailsById for {RequestId}", trid);
                throw;
            }
        }

        public async Task<TravelRequest> getUpdateRequestStatus(TravelRequest travelRequest)
        {
            try
            {
                _context.Entry(travelRequest).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return await _context.TravelRequests
                                     .Include(tr => tr.Location)
                                     .Include(tr => tr.TravelBudgetAllocations)
                                     .FirstAsync(tr => tr.RequestId == travelRequest.RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in getUpdateRequestStatus for {RequestId}", travelRequest.RequestId);
                throw;
            }
        }
    }
}
