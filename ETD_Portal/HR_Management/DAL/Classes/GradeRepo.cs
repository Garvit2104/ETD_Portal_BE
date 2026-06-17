using ETD_Portal.Data;
using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ETD_Portal.HR_Management.DAL.Classes
{
    public class GradeRepo : IGradeRepo
    {
        private readonly ETDPortalDbContext context;

        private readonly ILogger<GradeRepo> _logger;
        public GradeRepo(ETDPortalDbContext context, ILogger<GradeRepo> logger)
        {
            this.context = context;
            this._logger = logger;
        }
        public async Task<IEnumerable<Grade>> GetAllGrades()
        {
            try
            {
                return await this.context.Grades.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while fetching all grades");
                throw;
            }
        }
    }
}
