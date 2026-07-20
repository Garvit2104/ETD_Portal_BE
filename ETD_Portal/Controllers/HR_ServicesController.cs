using ETD_Portal.HR_Management.BLL.Interfaces;
using ETD_Portal.HR_Management.DTOs.RequestDTO;
using ETD_Portal.HR_Management.DTOs.ResponseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static ETD_Portal.HR_Management.BLL.Classes.UserServices;

namespace ETD_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR")]
    public class HR_ServicesController : ControllerBase
    {
        private readonly IGradeServices _gradeServices;
        private readonly IUserServices _userService;
        private readonly ILogger<HR_ServicesController> _logger;

        public HR_ServicesController(IGradeServices gradeServices, IUserServices userService, ILogger<HR_ServicesController> logger)
        {
            this._gradeServices = gradeServices;
            this._userService = userService;
            this._logger = logger;
        }

        [HttpGet("grades")]
        public async Task<ActionResult<IEnumerable<GradesResponseDTO>>> GetAllGrades()
        {
            _logger.LogInformation("Fetching all grades.");
            try
            {
                var grades = await _gradeServices.GetAllGrades();
                if (grades is null || !grades.Any())
                {
                    _logger.LogWarning("No grades found in the system");
                    return NotFound("No grades found.");
                }
                    
                return Ok(grades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching grades");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("employees")]
        public async Task<ActionResult<UserResponseDTO>> AddEmployee(UserRequestDTO userRequestDTO)
        {
            _logger.LogInformation("Add employee request received for {Email} with role {Role}",
        userRequestDTO?.email_address, userRequestDTO?.role);
            try
            {
                if (userRequestDTO == null)
                {
                    _logger.LogWarning("Add employee called with null request body");
                    return BadRequest("Employee data cannot be null.");
                }

                var result = await _userService.AddEmployee(userRequestDTO);
                return StatusCode(201, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation failed while adding employee {Email}: {Reason}",
                userRequestDTO?.email_address, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding employee {Email}",
                userRequestDTO?.email_address);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAllEmployees()
        {
            _logger.LogInformation("Fetching all employees");
            try
            {
                var result = await _userService.GetAllEmployess();
                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No employees found in the system");
                    return NotFound("No employees found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching employees");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("employees/{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetEmployeeById(int id)
        {
            _logger.LogInformation("Fetching employee with {EmployeeId}", id);
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid employee ID: {EmployeeId}", id);
                    return BadRequest("Invalid employee ID.");
                }
                var result = await _userService.GetEmployeeById(id);

                if(result is null)
                {
                    _logger.LogWarning("Employee with ID {EmployeeId} not found", id);
                    return NotFound($"Employee with ID {id} not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching employee with {EmployeeId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("employees/{id}")]
        public async Task<ActionResult<UserResponseDTO>> UpdateEmployeeById(int id, GradeUpdateRequestDTO updateRequestDTO)
        {
            _logger.LogInformation("Update employee request received for {EmployeeId} with proposed grade {ProposedGradeId}",
                id, updateRequestDTO?.current_grade_id);

            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Update employee called with invalid id {EmployeeId}", id);
                    return BadRequest("Invalid employee ID.");
                }

                if (updateRequestDTO == null)
                {
                    _logger.LogWarning("Update employee called with null request body for {EmployeeId}", id);
                    return BadRequest("Employee data cannot be null.");
                }

                var result = await _userService.UpdateEmployeeById(id, updateRequestDTO);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation failed while updating employee {EmployeeId}: {Reason}",
                    id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (GradeUpdateRuleViolationException ex)
            {
                _logger.LogWarning("Grade rule violation while updating employee {EmployeeId}: {Reason}",
                    id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Employee not found while updating {EmployeeId}: {Reason}", id, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating employee {EmployeeId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("employees/{id}")]
        public async Task<IActionResult> DeleteEmployeeById(int id)
        {
            _logger.LogInformation("Delete employee request received for {EmployeeId}", id);

            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Delete employee called with invalid id {EmployeeId}", id);
                    return BadRequest("Invalid employee ID.");
                }

                await _userService.DeleteEmployeeById(id);
                return Ok($"Employee with ID {id} deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Employee not found while deleting {EmployeeId}: {Reason}", id, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting employee {EmployeeId}", id);
                var fullError = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, fullError);
            }
        }
    }
}
