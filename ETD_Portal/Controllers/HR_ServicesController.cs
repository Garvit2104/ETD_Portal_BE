using ETD_Portal.HR_Management.BLL.Interfaces;
using ETD_Portal.HR_Management.DTOs.RequestDTO;
using ETD_Portal.HR_Management.DTOs.ResponseDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETD_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HR_ServicesController : ControllerBase
    {
        private readonly IGradeServices _gradeServices;
        private readonly IUserServices _userService;

        public HR_ServicesController(IGradeServices gradeServices, IUserServices userService)
        {
            this._gradeServices = gradeServices;
            this._userService = userService;
        }

        [HttpGet("grades")]
        public async Task<ActionResult<IEnumerable<GradesResponseDTO>>> GetAllGrades()
        {
            try
            {
                var grades = await _gradeServices.GetAllGrades();
                if (grades == null || !grades.Any())
                    return NotFound("No grades found.");
                return Ok(grades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("employees")]
        public async Task<ActionResult<UserResponseDTO>> AddEmployee(UserRequestDTO userRequestDTO)
        {
            try
            {
                if (userRequestDTO == null)
                    return BadRequest("Employee data cannot be null.");
                var result = await _userService.AddEmployee(userRequestDTO);
                return StatusCode(201, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAllEmployees()
        {
            try
            {
                var result = await _userService.GetAllEmployess();
                if (result == null || !result.Any())
                    return NotFound("No employees found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("employees/{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetEmployeeById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid employee ID.");
                var result = await _userService.GetEmployeeById(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("employees/{id}")]
        public async Task<ActionResult<UserResponseDTO>> UpdateEmployeeById(int id, UserRequestDTO userRequestDTO)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid employee ID.");
                if (userRequestDTO == null)
                    return BadRequest("Employee data cannot be null.");
                var result = await _userService.updateEmployeeById(id, userRequestDTO);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("employees/{id}")]
        public async Task<IActionResult> DeleteEmployeeById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid employee ID.");
                await _userService.DeleteEmployeeById(id);
                return Ok($"Employee with ID {id} deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
