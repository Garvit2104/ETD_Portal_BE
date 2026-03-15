using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reimbursement__Managment.Data;
using Reimbursement__Managment.DTOs.Reimbursement;
using Reimbursement__Managment.Models;

namespace Reimbursement__Managment.DAL.Reimbursements
{
    public class ReimbursementRepoClass : IReimbursementRepo
    {
        private readonly ReimbursementDbContext _context;
        public ReimbursementRepoClass(ReimbursementDbContext _context)
        {
            this._context = _context;
        }
           

        public async Task<ReimbursementRequest> AddReimbursement(ReimbursementRequest entity)
        {
            var saveReimburseRequest = await _context.ReimbursementRequests.AddAsync(entity);
            // SaveChangesAsync commits to DB and EF auto-populates the ID
            await _context.SaveChangesAsync();
            return saveReimburseRequest.Entity;  
        }

        public async Task<IEnumerable<ReimbursementRequest>> GetAllReimbursementRequest(int travelrequestid)
        {
            var allRequests =  _context.ReimbursementRequests.AsNoTracking().Where(tr => tr.TravelRequestId == travelrequestid).AsEnumerable();
            return await Task.FromResult(allRequests);
        }
    }
}
