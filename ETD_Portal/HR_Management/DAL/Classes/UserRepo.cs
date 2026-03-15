using ETD_Portal.HR_Management.DAL.Interfaces;

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
            var savedUser = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return savedUser.Entity;
        }

        public async Task<IEnumerable<User>> GetAllEmployee()
        {
            return await _context.Users
                                 .Include(u => u.CurrentGrade)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<User> GetEmployeeById(int? employeeId)
        {
            var empData = await _context.Users
                                        .Include(u => u.CurrentGrade)
                                        .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);
            if (empData == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
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
            var employee = await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found.");

            // Delete related grade histories first to avoid FK constraint error
            var gradeHistories = _context.GradeHistories.Where(gh => gh.EmployeeId == id);
            _context.GradeHistories.RemoveRange(gradeHistories);
            _context.Users.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
