using ETD_Portal.Data;
using ETD_Portal.Models;
using ETD_Portal.Reimbursement_Mgmt.BLL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.DAL.Classes
{
    public  class ReimbursementRepo : IReimbursementRepo
    {
        private readonly ETDPortalDbContext _context;
        private readonly ILogger<ReimbursementRepo> _logger;
        public ReimbursementRepo(ETDPortalDbContext _context, ILogger<ReimbursementRepo> logger)
        {
            this._context = _context;
            _logger = logger;
        }
        public async Task<ReimbursementRequest> AddReimbursement(ReimbursementRequest entity)
        {
            try
            {
                var saveReimburseRequest = await _context.ReimbursementRequests.AddAsync(entity);
                // SaveChangesAsync commits to DB and EF auto-populates the ID
                await _context.SaveChangesAsync();
                var savedReimbursement = await _context.ReimbursementRequests
                                                       .Include(u => u.ReimbursementType)
                                                       .FirstAsync(u => u.Id == entity.Id);
                _logger.LogInformation("Saved reimbursement request with Id {ReimbursementId}", entity.Id);
                return savedReimbursement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving reimbursement request for TravelRequestId {TravelRequestId}", entity.TravelRequestId);
                throw;
            }

        }

        public async Task<List<ReimbursementRequest>> GetAllReimbursementRequest(int trid)
        {
            try
            {
                var result = await _context.ReimbursementRequests.Where(tr => tr.TravelRequestId == trid).ToListAsync();
                _logger.LogInformation("Fetched {Count} reimbursement requests for TravelRequestId {TravelRequestId}", result.Count, trid);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching reimbursement requests for TravelRequestId {TravelRequestId}", trid);
                throw;
            }
        }

        public async Task<ReimbursementRequest> GetReimbursementDetails(int reimbursementid)
        {
            try
            {
                var reimburseDetails = await _context.ReimbursementRequests.AsNoTracking()
                                                     .FirstOrDefaultAsync(r => r.Id == reimbursementid);
                if (reimburseDetails == null)
                {
                    throw new KeyNotFoundException($"Reimbursement with ID {reimbursementid} not found.");
                }
                _logger.LogInformation("Fetched reimbursement details for ReimbursementId {ReimbursementId}", reimbursementid);
                return reimburseDetails;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching reimbursement details for ReimbursementId {ReimbursementId}", reimbursementid);
                throw;
            }
        }

        public async Task<ReimbursementRequest> ProcessReimbursemnet(ReimbursementRequest reimbursementRequest)
        {
            try
            {
                _context.Entry(reimbursementRequest).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                var updatedReimbursement = await _context.ReimbursementRequests
                                                         .Include(r => r.ReimbursementType)
                                                         .FirstAsync(tr => tr.Id == reimbursementRequest.Id);
                _logger.LogInformation("Processed reimbursement request with Id {ReimbursementId}, Status {Status}",
                    reimbursementRequest.Id, reimbursementRequest.Status);
                return updatedReimbursement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing reimbursement request with Id {ReimbursementId}", reimbursementRequest.Id);
                throw;
            }
        }
    }
}
