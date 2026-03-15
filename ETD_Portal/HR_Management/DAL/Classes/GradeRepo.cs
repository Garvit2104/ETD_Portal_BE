using ETD_Portal.HR_Management.DAL.Interfaces;
using System.Diagnostics;

namespace ETD_Portal.HR_Management.DAL.Classes
{
    public class GradeRepo : IGradeRepo
    {
        private readonly ETD_PortalDbContext context;
        public GradeRepo(ETD_PortalDbContext context)
        {
               this.context = context;
        }
        public async Task<IEnumerable<Grade>> GetAllGrades()
        {
            var data = this.context.Grades.AsNoTracking().AsEnumerable();
            return await Task.FromResult(data);
        }

    }
}
