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
        public GradeRepo(ETDPortalDbContext context)
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
