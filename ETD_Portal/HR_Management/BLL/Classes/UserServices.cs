using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.HR_Management.DTOs.RequestDTO;
using ETD_Portal.HR_Management.DTOs.ResponseDTO;

namespace ETD_Portal.HR_Management.BLL.Classes
{
    public class UserServices : IUserServices
    {

        private readonly IUserRepo _userRepo;
        private readonly IGradeHistoryRepo _gradeHistoryRepo;

        public UserServices(IGradeHistoryRepo gradeHistoryRepo, IUserRepo userRepo)
        {
            this._userRepo = userRepo;
            this._gradeHistoryRepo = gradeHistoryRepo;
        }

        public class GradeUpdateRuleViolationException : Exception
        {
            public GradeUpdateRuleViolationException()
                : base("Grade Update Rule Violation Exception") { }
        }

        // Maps UserRequestDTO → User entity
        public static User MapUserRequestDTOtoEntity(UserRequestDTO user)
        {
            return new User
            {
                FirstName = user.first_name,
                LastName = user.last_name,
                PhoneNumber = user.phone_number,
                EmailAddress = user.email_address,
                Role = user.role,
                CurrentGradeId = user.current_grade_id
            };
        }

        // Maps User entity → UserResponseDTO
        public async Task<UserResponseDTO> MapEntityResponseToUserResponseDTO(User user)
        {
            return new UserResponseDTO
            {
                employee_id = user.EmployeeId,
                first_name = user.FirstName,
                last_name = user.LastName,
                phone_number = user.PhoneNumber,
                email_address = user.EmailAddress,
                role = user.Role,
                current_grade_id = user.CurrentGrade?.Name
            };
        }

        // Validates business rules
        private static void ValidateEmployee(UserRequestDTO userRequestDTO)
        {
            if (string.IsNullOrWhiteSpace(userRequestDTO.email_address) ||
                !userRequestDTO.email_address.EndsWith("@Conzy.com", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Email address must be in the format xxxx@Conzy.com");

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

            // Step 3: TravelDeskExe default grade is Grade-1
            if (userEntity.Role.Equals("TravelDeskExe", StringComparison.OrdinalIgnoreCase))
                userEntity.CurrentGradeId = 1;

            // Step 4: Save employee
            var savedUser = await _userRepo.AddEmployee(userEntity);

            // Step 5: Save initial grade history
            var gradeHistory = new GradeHistory
            {
                AssignedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                EmployeeId = savedUser.EmployeeId,
                GradeId = savedUser.CurrentGradeId
            };
            await _gradeHistoryRepo.AddGradeHistory(gradeHistory);

            // Step 6: Return response DTO
            return await MapEntityResponseToUserResponseDTO(savedUser);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllEmployess()
        {
            var result = await _userRepo.GetAllEmployee();
            var userList = new List<UserResponseDTO>();
            foreach (var item in result)
                userList.Add(await MapEntityResponseToUserResponseDTO(item));
            return userList;
        }

        public async Task<UserResponseDTO> GetEmployeeById(int employeeId)
        {
            var empData = await _userRepo.GetEmployeeById(employeeId);
            return await MapEntityResponseToUserResponseDTO(empData);
        }

        public async Task<UserResponseDTO> updateEmployeeById(int id, UserRequestDTO userRequestDTO)
        {
            // Step 1: Validate
            ValidateEmployee(userRequestDTO);

            // Step 2: Fetch existing employee
            var empData = await _userRepo.GetEmployeeById(id);

            int? currentGradeId = empData.CurrentGradeId;
            int? newGradeId = userRequestDTO.current_grade_id;

            // Step 3: Check downgrade — Grade-1 is highest so newGradeId > currentGradeId means downgrade
            if (newGradeId > currentGradeId)
                throw new ArgumentException("Employee grade cannot be downgraded.");

            // Step 4: Update fields
            empData.FirstName = userRequestDTO.first_name;
            empData.LastName = userRequestDTO.last_name;
            empData.PhoneNumber = userRequestDTO.phone_number;
            empData.EmailAddress = userRequestDTO.email_address;
            empData.Role = userRequestDTO.role;
            empData.CurrentGradeId = userRequestDTO.current_grade_id;

            // Step 5: Grade history rules if grade changed
            if (newGradeId != currentGradeId)
            {
                var gradeHistories = await _gradeHistoryRepo
                                        .GetAllGradeHistoryByEmployeeId(empData.EmployeeId);
                var historyList = gradeHistories.OrderBy(gh => gh.AssignedOn).ToList();

                var firstAssignedOn = historyList.First().AssignedOn!.ToDateTime(TimeOnly.MinValue);
                var lastAssignedOn = historyList.Last().AssignedOn!.ToDateTime(TimeOnly.MinValue);
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
                await _gradeHistoryRepo.AddGradeHistory(newGradeHistory);
            }

            // Step 6: Save to DB
            await _userRepo.updateEmployeeById(empData);

            // Step 7: Return response
            return await MapEntityResponseToUserResponseDTO(empData);
        }

        public async Task<bool> DeleteEmployeeById(int id)
        {
            var empData = await _userRepo.GetEmployeeById(id);
            return await _userRepo.DeleteEmployeeById(id);
        }
    }
}
