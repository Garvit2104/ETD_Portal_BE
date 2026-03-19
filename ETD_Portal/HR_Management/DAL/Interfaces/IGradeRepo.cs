using ETD_Portal.Models;
using System.Diagnostics;

namespace ETD_Portal.HR_Management.DAL.Interfaces
{
    public interface IGradeRepo
    {
        public Task<IEnumerable<Grade>> GetAllGrades();

    }
}
