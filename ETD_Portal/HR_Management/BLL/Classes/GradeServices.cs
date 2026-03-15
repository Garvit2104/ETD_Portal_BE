using ETD_Portal.HR_Management.BLL.Interfaces;
using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.HR_Management.DTOs.ResponseDTO;

namespace ETD_Portal.HR_Management.BLL.Classes
{
    public class GradeServices : IGradeServices
    {
        private readonly IGradeRepo _gradeRepo;

        public GradeServices(IGradeRepo gradeRepo)
        {
            this._gradeRepo = gradeRepo;
        }

        public async Task<IEnumerable<GradesResponseDTO>> GetAllGrades()
        {
            var grades = await _gradeRepo.GetAllGrades();
            return grades.Select(g => new GradesResponseDTO
            {
                id = g.Id,
                name = g.Name
            });
        }
    
    }
}
