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
        private readonly ILogger<ReimbursementTypeRepo> _logger;
        public ReimbursementTypeRepo(ETDPortalDbContext _context, ILogger<ReimbursementTypeRepo> logger)
        {
            this._context = _context;
            _logger = logger;
        }
        public async Task<List<ReimbursementType>> GetAllReimbursementType()
        {
            try
            {
                var types = await _context.ReimbursementTypes.AsNoTracking().ToListAsync();
                _logger.LogInformation("Fetched {Count} reimbursement types", types.Count);
                return await Task.FromResult(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching reimbursement types from the database");
                throw;
            }
        }


        // In ReimbursementTypeRepository
        public async Task<ReimbursementType> GetTypeById(int id)
        {
            try
            {
                var reimbursementType = await _context.ReimbursementTypes
                                                      .AsNoTracking()
                                                      .FirstOrDefaultAsync(t => t.Id == id);
                _logger.LogInformation("Fetched reimbursement type for Id {ReimbursementTypeId}", id);
                return reimbursementType;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching reimbursement type for Id {ReimbursementTypeId}", id);
                throw;
            }
        }
    }
}
