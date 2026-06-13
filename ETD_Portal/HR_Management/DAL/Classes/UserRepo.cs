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
        private readonly ILogger<UserRepo> _logger;

        public UserRepo(ETDPortalDbContext context, ILogger<UserRepo> _logger)
        {
            this._context = context;
            _logger = _logger;
        }

        public async Task<User> AddEmployee(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                await _context.Entry(user).Reference(u => u.CurrentGrade).LoadAsync();

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while adding employee {EmployeeId}", user?.EmployeeId);
                throw;
            }


        }

        public async Task<IEnumerable<User>> GetAllEmployee()
        {
            try
            {
                return await _context.Users
                                     .Include(u => u.CurrentGrade)
                                     .AsNoTracking()
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while fetching all employees");
                throw;
            }
        }

        public async Task<User> GetEmployeeById(int? employeeId)
        {
            try
            {
                var empData = await _context.Users
                                            .Include(u => u.CurrentGrade)
                                            .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);

                return empData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while fetching employee with {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<bool> UpdateEmployeeById(User user)
        {
            if (user == null)
            return false;

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while updating employee {EmployeeId}", user.EmployeeId);
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeById(int id)
        {
            try
            {
                var employee = await _context.Users
                    .Include(u => u.GradeHistories)
                    .FirstOrDefaultAsync(u => u.EmployeeId == id);

                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {id} not found.");

                var travelRequests = await _context.TravelRequests
                    .Where(tr => tr.RaisedByEmployeeId == id)
                    .ToListAsync();
                _context.TravelRequests.RemoveRange(travelRequests);

                // Delete related grade histories first to avoid FK constraint error
                _context.GradeHistories.RemoveRange(employee.GradeHistories);

                _context.Users.Remove(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (KeyNotFoundException)
            {
                // Not-found is a business outcome, not a DB error — let it bubble up unlogged.
                // Controller will Warning-log it.
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while deleting employee {EmployeeId}", id);
                throw;
            }
        }


    }
}
