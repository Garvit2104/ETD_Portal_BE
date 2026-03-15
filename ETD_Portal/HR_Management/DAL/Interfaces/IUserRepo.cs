namespace ETD_Portal.HR_Management.DAL.Interfaces
{
    public interface IUserRepo
    {
        Task<User> AddEmployee(User user);

        public Task<IEnumerable<User>> GetAllEmployee();

        Task<User> GetEmployeeById(int employeeId);

        Task<bool> updateEmployeeById(User user);

        Task<Boolean> DeleteEmployeeById(int id);
    }
}
