using ETD_Portal.TravelPlanner.BLL.Classes;
using ETD_Portal.TravelPlanner.BLL.Interfaces;
using ETD_Portal.TravelPlanner.DTOs.RequestDTO;
using ETD_Portal.TravelPlanner.DTOs.ResponseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETD_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TP_ServicesController : ControllerBase
    {
        private readonly ILocationServices _locationService;
        private readonly ITravelRequestServices _travelRequestService;
        private readonly IBudgetAllocationServices _budgetAllocationServices;
        private readonly ILogger<TP_ServicesController> _logger;

        public TP_ServicesController(
            ILocationServices locationService,
            ITravelRequestServices travelRequestService,
            IBudgetAllocationServices budgetAllocationServices,
            ILogger<TP_ServicesController> logger   )
        {
            this._locationService = locationService;
            this._travelRequestService = travelRequestService;
            this._budgetAllocationServices = budgetAllocationServices;
            _logger = logger;
        }

        [HttpGet("locations")]
        [Authorize(Roles = "Employee,HR,TravelDeskExec")]
        public async Task<ActionResult<IEnumerable<LocationResponseDTO>>> GetAllLocations()
        {
            _logger.LogInformation("GetAllLocations called");
            try
            {
                var locations = await _locationService.GetAllLocation();
                if(locations is null || !locations.Any())
                {
                    _logger.LogWarning("No location found in the system");
                }
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetAllLocations");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("travelrequests/new")]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<TravelResponseDTO>> CreateTravelRequest(TravelRequestDTO tRequestDTO)
        {
            _logger.LogInformation("CreateTravelRequest called for {EmployeeId}", tRequestDTO.raised_by_employee_id);
            try
            {
                var result = await _travelRequestService.CreateTravelRequest(tRequestDTO);
                return StatusCode(201, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("CreateTravelRequest validation failed for {EmployeeId}: {Message}", tRequestDTO.raised_by_employee_id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("CreateTravelRequest entity not found for {EmployeeId}: {Message}", tRequestDTO.raised_by_employee_id, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CreateTravelRequest for {EmployeeId}", tRequestDTO.raised_by_employee_id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("travelrequests/{hrId}/pending")]
        [Authorize(Roles = "HR")]
        public async Task<ActionResult<IEnumerable<TravelResponseDTO>>> GetAllPendingRequests(int hrId)
        {
            _logger.LogInformation("GetAllPendingRequests called for {HRId}", hrId);
            try
            {
                var result = await _travelRequestService.GetAllPendingRequests(hrId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("GetAllPendingRequests validation failed for {HRId}: {Message}", hrId, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("GetAllPendingRequests entity not found for {HRId}: {Message}", hrId, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetAllPendingRequests for {HRId}", hrId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("travelrequests/{trid}")]
        [Authorize(Roles = "Employee,HR,TravelDeskExec")]
        public async Task<ActionResult<TravelRequestDetailsRespDTO>> GetTravelRequestById(int trid)
        {
            _logger.LogInformation("GetTravelRequestById called for {RequestId}", trid);
            try
            {
                var result = await _travelRequestService.GetTravelRequestDetailsById(trid);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("GetTravelRequestById entity not found for {RequestId}: {Message}", trid, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetTravelRequestById for {RequestId}", trid);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("travelrequests/{trid}/update")]
        [Authorize(Roles = "HR")]
        public async Task<ActionResult<TravelRequestDetailsRespDTO>> UpdateRequestStatus(int trid, UpdateRequestDTO updateDTO)
        {
            _logger.LogInformation("UpdateRequestStatus called for {RequestId}", trid);
            try
            {
                var result = await _travelRequestService.UpdateRequestStatus(trid, updateDTO);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("UpdateRequestStatus validation failed for {RequestId}: {Message}", trid, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("UpdateRequestStatus entity not found for {RequestId}: {Message}", trid, ex.Message);
                return NotFound(ex.Message);
            }
    
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UpdateRequestStatus for {RequestId}", trid);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("travelrequests/calculatebudget")]
        [Authorize(Roles = "HR")]
        public async Task<ActionResult<int>> CalculateBudget([FromQuery] int travelRequestId)
        {
            _logger.LogInformation("CalculateBudget called for {TravelRequestId}", travelRequestId);
            try
            {
                var budget = await _budgetAllocationServices.CalculateBudget(travelRequestId);
                return Ok(budget);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("CalculateBudget validation failed for {TravelRequestId}: {Message}", travelRequestId, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("CalculateBudget entity not found for {TravelRequestId}: {Message}", travelRequestId, ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("CalculateBudget invalid operation for {TravelRequestId}: {Message}", travelRequestId, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CalculateBudget for {TravelRequestId}", travelRequestId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
