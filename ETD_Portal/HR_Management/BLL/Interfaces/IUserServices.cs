using ETD_Portal.HR_Management.DTOs.RequestDTO;
using ETD_Portal.HR_Management.DTOs.ResponseDTO;
using ETD_Portal.Models;

namespace ETD_Portal.HR_Management.BLL.Interfaces
{
    public interface IUserServices
    {
        Task<UserResponseDTO> AddEmployee(UserRequestDTO userRequestDTO);
        Task<IEnumerable<UserResponseDTO>> GetAllEmployess();
        Task<UserResponseDTO> GetEmployeeById(int employeeId);
        Task<UserResponseDTO> updateEmployeeById(int id, UserRequestDTO userRequestDTO);
        Task<bool> DeleteEmployeeById(int id);
        
    }
}
