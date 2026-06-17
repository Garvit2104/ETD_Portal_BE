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

                // Collect all travel request IDs raised by this employee
                var travelRequestIds = await _context.TravelRequests
                    .Where(tr => tr.RaisedByEmployeeId == id)
                    .Select(tr => tr.RequestId)
                    .ToListAsync();

                if (travelRequestIds.Count > 0)
                {
                    // ReservationDocs — deepest child, must be removed before Reservations
                    var reservationIds = await _context.Reservations
                        .Where(r => travelRequestIds.Contains(r.TravelRequestId))
                        .Select(r => r.Id)
                        .ToListAsync();

                    if (reservationIds.Count > 0)
                    {
                        var reservationDocs = await _context.ReservationDocs
                            .Where(d => d.ReservationId != null && reservationIds.Contains(d.ReservationId.Value))
                            .ToListAsync();
                        _context.ReservationDocs.RemoveRange(reservationDocs);
                    }

                    // Reservations
                    var reservations = await _context.Reservations
                        .Where(r => travelRequestIds.Contains(r.TravelRequestId))
                        .ToListAsync();
                    _context.Reservations.RemoveRange(reservations);

                    // Reimbursement requests
                    var reimbursements = await _context.ReimbursementRequests
                        .Where(r => travelRequestIds.Contains(r.TravelRequestId))
                        .ToListAsync();
                    _context.ReimbursementRequests.RemoveRange(reimbursements);

                    // Budget allocations
                    var budgetAllocations = await _context.TravelBudgetAllocations
                        .Where(b => travelRequestIds.Contains(b.TravelRequestId))
                        .ToListAsync();
                    _context.TravelBudgetAllocations.RemoveRange(budgetAllocations);

                    // Travel requests
                    var travelRequests = await _context.TravelRequests
                        .Where(tr => travelRequestIds.Contains(tr.RequestId))
                        .ToListAsync();
                    _context.TravelRequests.RemoveRange(travelRequests);
                }

                // Refresh tokens (ClientSetNull behavior won't work without tracking — delete explicitly)
                var refreshTokens = await _context.RefreshTokens
                    .Where(rt => rt.EmployeeId == id)
                    .ToListAsync();
                _context.RefreshTokens.RemoveRange(refreshTokens);

                // Grade histories (already loaded via Include)
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
