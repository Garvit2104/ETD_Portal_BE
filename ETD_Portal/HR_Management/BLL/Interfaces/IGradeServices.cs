using ETD_Portal.HR_Management.DTOs.ResponseDTO;

namespace ETD_Portal.HR_Management.BLL.Interfaces
{
    public interface IGradeServices
    {
        Task<IEnumerable<GradesResponseDTO>> GetAllGrades();
    }
}
