using ETD_Portal.Data;
using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.Models;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.HR_Management.DAL.Classes
{
    public class GradeHistoryRepo : IGradeHistoryRepo
    {
        private readonly ETDPortalDbContext _context;

        public GradeHistoryRepo(ETDPortalDbContext context)
        {
            this._context = context;
        }

        public async Task AddGradeHistory(GradeHistory gradeHistory)
        {
            await _context.GradeHistories.AddAsync(gradeHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GradeHistory>> GetAllGradeHistoryByEmployeeId(int? id)
        {
            return await _context.GradeHistories
                                 .Where(gh => gh.EmployeeId == id)
                                 .OrderBy(gh => gh.AssignedOn)
                                 .ToListAsync();
        }
    }
}
