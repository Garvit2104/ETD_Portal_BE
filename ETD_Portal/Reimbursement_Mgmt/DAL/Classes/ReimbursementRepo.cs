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
        public ReimbursementRepo(ETDPortalDbContext _context)
        {
            this._context = _context;
        }
        public async Task<ReimbursementRequest> AddReimbursement(ReimbursementRequest entity)
        {
            var saveReimburseRequest = await _context.ReimbursementRequests.AddAsync(entity);
            // SaveChangesAsync commits to DB and EF auto-populates the ID
            await _context.SaveChangesAsync();
            return await _context.ReimbursementRequests.Include(u => u.ReimbursementType)
                                                .FirstAsync(u => u.Id == entity.Id);
           
        }

        public async Task<List<ReimbursementRequest>> GetAllReimbursementRequest(int trid)
        {
            var result = await _context.ReimbursementRequests.Where(tr => tr.TravelRequestId == trid).ToListAsync();
            return result;
        }

        public async Task<ReimbursementRequest> GetReimbursementDetails(int reimbursementid)
        {
            return await _context.ReimbursementRequests.AsNoTracking()
                              .FirstOrDefaultAsync(r => r.Id == reimbursementid);
        }

        public async Task<ReimbursementRequest> ProcessReimbursemnet(ReimbursementRequest reimbursementRequest)
        {
            _context.Entry(reimbursementRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return await _context.ReimbursementRequests.FirstAsync(tr => tr.Id == reimbursementRequest.Id);
        }
    }
}
