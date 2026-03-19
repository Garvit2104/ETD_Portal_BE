using ETD_Portal.Data;
using ETD_Portal.Models;
using ETD_Portal.TravelPlanner.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.TravelPlanner.DAL.Classes
{
    public class TravelRequestRepo : ITravelRequestRepo
    {
        private readonly ETDPortalDbContext _context;

        public TravelRequestRepo(ETDPortalDbContext context)
        {
            this._context = context;
        }

        public async Task<TravelRequest> CreateTravelRequest(TravelRequest travelRequest)
        {
            var result = await _context.TravelRequests.AddAsync(travelRequest);
            await _context.SaveChangesAsync();
            return await _context.TravelRequests
                                 .Include(t => t.Location)
                                 .FirstAsync(t => t.RequestId == travelRequest.RequestId);
        }

        public async Task<IEnumerable<TravelRequest>> GetAllPendingRequests(int hrId)
        {
            return await _context.TravelRequests
                                 .Include(tr => tr.Location)
                                 .Where(tr => tr.ToBeApprovedByHrId == hrId
                                           && tr.RequestStatus == "New")
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<TravelRequest> getTravelRequestById(int trid)
        {
            return await _context.TravelRequests
                                       .Include(tr => tr.Location)
                                       .FirstOrDefaultAsync(tr => tr.RequestId == trid);
      
        }

        public async Task<TravelRequest> getUpdateRequestStatus(TravelRequest travelRequest)
        {
            _context.Entry(travelRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return await _context.TravelRequests
                                 .Include(tr => tr.Location)
                                 .FirstAsync(tr => tr.RequestId == travelRequest.RequestId);
        }
    }
}
