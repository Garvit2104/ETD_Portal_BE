using ETD_Portal.HR_Management.BLL.Interfaces;
using ETD_Portal.HR_Management.DAL.Classes;
using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.HR_Management.DTOs.RequestDTO;
using ETD_Portal.HR_Management.DTOs.ResponseDTO;
using ETD_Portal.Models;

namespace ETD_Portal.HR_Management.BLL.Classes
{
    public class UserServices : IUserServices
    {

        private readonly IUserRepo userRepo;
        private readonly IGradeHistoryRepo gradeHistoryRepo;
        private readonly IGradeRepo gradesRepo;

        public UserServices(IGradeHistoryRepo gradeHistoryRepo, IUserRepo userRepo, IGradeRepo gradesRepo)
        {
            this.userRepo = userRepo;
            this.gradeHistoryRepo = gradeHistoryRepo;
            this.gradesRepo = gradesRepo;
        }

        public class GradeUpdateRuleViolationException : Exception
        {
            public GradeUpdateRuleViolationException()
                : base("Grade Update Rule Violation Exception") { }
        }

        // Maps UserRequestDTO → User entity
        public async Task<UserResponseDTO> MapEntityResponseToUserResponseDTO(User user)
        {

            //Grade grade = await gradesRepo.GetGradeById(user.CurrentGradeId.Value

            return new UserResponseDTO
            {
                employee_id = user.EmployeeId,
                first_name = user.FirstName,
                last_name = user.LastName,
                phone_number = user.PhoneNumber,
                email_address = user.EmailAddress,
                role = user.Role,
                current_grade_id = user.CurrentGrade?.Id,
                current_grade_name = user.CurrentGrade?.Name ?? "Not Assigned"
            };
        }

        public static User MapUserRequestDTOtoEntity(UserRequestDTO user)
        {
            return new User
            {

                FirstName = user.first_name,
                LastName = user.last_name,
                PhoneNumber = user.phone_number,
                EmailAddress = user.email_address,
                Role = user.role,
                CurrentGradeId = user.current_grade_id,
               

            };
        }

        // Validates business rules
        private static void ValidateEmployee(UserRequestDTO userRequestDTO)
        {
            if (string.IsNullOrWhiteSpace(userRequestDTO.password_hash))
                throw new ArgumentException("Password is required when adding a new employee.");

            if (userRequestDTO.password_hash.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters.");

            if (string.IsNullOrWhiteSpace(userRequestDTO.email_address) ||
                !userRequestDTO.email_address.EndsWith("@cognizant.com", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Email address must be in the format xxxx@cognizant.com");

            if (string.IsNullOrWhiteSpace(userRequestDTO.phone_number) ||
                userRequestDTO.phone_number.Length != 10 ||
                !userRequestDTO.phone_number.All(char.IsDigit))
                throw new ArgumentException("Phone number must be exactly 10 digits.");

            var validRoles = new[] { "Employee", "HR", "TravelDeskExe" };
            if (!validRoles.Contains(userRequestDTO.role))
                throw new ArgumentException("Role must be Employee, HR or TravelDeskExe.");
        }

        public async Task<UserResponseDTO> AddEmployee(UserRequestDTO userRequestDTO)
        {
            // Step 1: Validate
            ValidateEmployee(userRequestDTO);

            // Step 2: Map DTO → entity
            var userEntity = MapUserRequestDTOtoEntity(userRequestDTO);

            userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRequestDTO.password_hash);

            // Step 3: TravelDeskExe default grade is Grade-1
            if (userEntity.Role.Equals("TravelDeskExe", StringComparison.OrdinalIgnoreCase))
                userEntity.CurrentGradeId = 1;

            // Step 4: Save employee
            var savedUser = await userRepo.AddEmployee(userEntity);

            // Step 5: Save initial grade history
            var gradeHistory = new GradeHistory
            {
                AssignedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                EmployeeId = savedUser.EmployeeId,
                GradeId = savedUser.CurrentGradeId
            };
            await gradeHistoryRepo.AddGradeHistory(gradeHistory);

            return await MapEntityResponseToUserResponseDTO(savedUser);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllEmployess()
        {
            var result = await userRepo.GetAllEmployee();
            var userList = new List<UserResponseDTO>();
            foreach (var item in result)
                userList.Add(await MapEntityResponseToUserResponseDTO(item));
            return userList;
        }

        public async Task<UserResponseDTO> GetEmployeeById(int? employeeId)
        {
            if (employeeId == null)
                throw new ArgumentNullException(nameof(employeeId), "Employee ID cannot be null.");

            var empData = await userRepo.GetEmployeeById(employeeId.Value);
            if(empData == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            return await MapEntityResponseToUserResponseDTO(empData);
        }

        public async Task<UserResponseDTO> UpdateEmployeeById(int id, GradeUpdateRequestDTO updateRequestDTO)
        {
            

            // Step 2: Fetch existing employee
            var empData = await userRepo.GetEmployeeById(id);

            if (empData == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found.");

            int? currentGradeId = empData.CurrentGradeId;
            int? newGradeId = updateRequestDTO.current_grade_id;

            // Step 3: Check downgrade — Grade-1 is highest so newGradeId > currentGradeId means downgrade
            if (newGradeId > currentGradeId)
                throw new ArgumentException("Employee grade cannot be downgraded.");

            // Step 4: Update fields
            empData.FirstName = updateRequestDTO.first_name;
            empData.LastName = updateRequestDTO.last_name;
            empData.PhoneNumber = updateRequestDTO.phone_number;
            empData.EmailAddress = updateRequestDTO.email_address;
            empData.Role = updateRequestDTO.role;
            empData.CurrentGradeId = updateRequestDTO.current_grade_id;

            // Step 5: Grade history rules if grade changed
            if (newGradeId != currentGradeId)
            {
                var gradeHistories = await gradeHistoryRepo
                                        .GetAllGradeHistoryByEmployeeId(empData.EmployeeId);
                var historyList = gradeHistories.OrderBy(gh => gh.AssignedOn).ToList();

                if (historyList.Count == 0)
                    throw new InvalidOperationException(
                        $"No grade history found for employee {id}. Cannot validate grade change rules.");

                var firstEntry = historyList.First();
                var lastEntry = historyList.Last();

                if (!firstEntry.AssignedOn.HasValue || !lastEntry.AssignedOn.HasValue)
                    throw new InvalidOperationException(
                        $"Grade history contains entries with missing dates for employee {id}.");

                var firstAssignedOn = firstEntry.AssignedOn.Value.ToDateTime(TimeOnly.MinValue);
                var lastAssignedOn = lastEntry.AssignedOn.Value.ToDateTime(TimeOnly.MinValue);
                var today = DateTime.Now;

                // Rule 1: Must complete 2 years before first grade change
                if ((today - firstAssignedOn).TotalDays < (2 * 365))
                    throw new GradeUpdateRuleViolationException();

                // Rule 2: Grade can only change once per year
                if ((today - lastAssignedOn).TotalDays < 365)
                    throw new GradeUpdateRuleViolationException();

                // Save new grade history
                var newGradeHistory = new GradeHistory
                {
                    AssignedOn = DateOnly.FromDateTime(today),
                    EmployeeId = empData.EmployeeId,
                    GradeId = newGradeId.Value
                };
                await gradeHistoryRepo.AddGradeHistory(newGradeHistory);
            }

            // Step 6: Save to DB
            await userRepo.UpdateEmployeeById(empData);

            // Step 7: Return response
            return await MapEntityResponseToUserResponseDTO(empData);
        }

        public async Task<bool> DeleteEmployeeById(int id)
        {
            var empData = await userRepo.DeleteEmployeeById(id);
            return empData;
        }
    }
}
