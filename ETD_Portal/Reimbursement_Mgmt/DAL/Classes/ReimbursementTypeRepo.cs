using ETD_Portal.Data;
using ETD_Portal.Models;
using ETD_Portal.Reimbursement_Mgmt.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.DAL.Classes
{
    public class ReimbursementTypeRepo : IReimbursementTypeRepo
    {
        private readonly ETDPortalDbContext _context;
        public ReimbursementTypeRepo(ETDPortalDbContext _context)
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
