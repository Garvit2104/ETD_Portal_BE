using ETD_Portal.HR_Management.DTOs.ResponseDTO;
using ETD_Portal.Models;

namespace ETD_Portal.HR_Management.DAL.Interfaces
{
    public interface IUserRepo
    {
        public Task<User> AddEmployee(User user);

        public Task<IEnumerable<User>> GetAllEmployee();

        Task<User> GetEmployeeById(int? employeeId);

        Task<bool> UpdateEmployeeById(User user);

        Task<Boolean> DeleteEmployeeById(int id);
    }
}
