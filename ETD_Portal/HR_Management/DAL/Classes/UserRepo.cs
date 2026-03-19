using ETD_Portal.Data;
using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.HR_Management.DTOs.ResponseDTO;
using ETD_Portal.Models;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.HR_Management.DAL.Classes
{
    public class UserRepo : IUserRepo
    {
        private readonly ETDPortalDbContext _context;

        public UserRepo(ETDPortalDbContext context)
        {
            this._context = context;
        }

        public async Task<User> AddEmployee(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return await _context.Users
                    .Include(u => u.CurrentGrade)
                    .FirstAsync(u => u.EmployeeId == user.EmployeeId);


        }

        public async Task<IEnumerable<User>> GetAllEmployee()
        {
            return await _context.Users
                                 .Include(u => u.CurrentGrade)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        
        public async Task<User> GetEmployeeById(int employeeId)
        {
            var empData = await _context.Users.Include(u => u.CurrentGrade).FirstOrDefaultAsync(U => U.EmployeeId == employeeId);

            if (empData == null)
            {
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            }

            return empData;
        }

        public async Task<bool> updateEmployeeById(User user)
        {
            if (user == null)
                return false;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEmployeeById(int id)
        {
            User employee = _context.Users.Find(id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found.");

            // Delete related grade histories first to avoid FK constraint error
            var gradeHistories = await _context.GradeHistories.Where(gh => gh.EmployeeId == id).ToListAsync();
            _context.GradeHistories.RemoveRange(gradeHistories);
            _context.Users.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        
    }
}
