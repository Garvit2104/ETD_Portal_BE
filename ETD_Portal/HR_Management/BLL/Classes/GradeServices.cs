using ETD_Portal.HR_Management.BLL.Interfaces;
using ETD_Portal.HR_Management.DAL.Classes;
using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.HR_Management.DTOs.ResponseDTO;

namespace ETD_Portal.HR_Management.BLL.Classes
{
    public class GradeServices : IGradeServices
    {
        private readonly IGradeRepo _gradeRepo;

        public GradeServices(IGradeRepo _gradeRepo)
        {
            this._gradeRepo = _gradeRepo;
        }

        public async Task<IEnumerable<GradeResponseDTO>> GetAllGrades()
        {
            var result = await _gradeRepo.GetAllGrades();

            List<GradeResponseDTO> ls = new List<GradeResponseDTO>();

            foreach (var item in result)
            {
                GradeResponseDTO gradeResponse = new GradeResponseDTO();
                gradeResponse.id = item.Id;
                gradeResponse.name = item.Name;
                
                ls.Add(gradeResponse);
            }
            return ls;
        }

    }
}
