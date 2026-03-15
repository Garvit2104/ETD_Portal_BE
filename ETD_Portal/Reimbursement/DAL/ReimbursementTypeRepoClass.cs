using Reimbursement__Managment.Models;
using Reimbursement__Managment.Data;
using Reimbursement__Managment.DAL;
using Microsoft.EntityFrameworkCore;
namespace Reimbursement__Managment.DAL
{
    
    public class ReimbursementTypeRepoClass : IReimbursementTypeRepo
    {
        private readonly ReimbursementDbContext _context;

        public ReimbursementTypeRepoClass(ReimbursementDbContext _context)
        {
            this._context = _context;
        }

        public async Task<List<ReimbursementType>> GetAllReimbursementType()
        {
            var types = await _context.ReimbursementTypes.AsNoTracking().ToListAsync();
            return await Task.FromResult(types);
        }

        
        // In ReimbursementTypeRepository
        public async Task<ReimbursementType> GetTypeById(int id)
        {
            return await _context.ReimbursementTypes
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
