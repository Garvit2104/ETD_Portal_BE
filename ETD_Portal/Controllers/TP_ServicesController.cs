using ETD_Portal.TravelPlanner.BLL.Classes;
using ETD_Portal.TravelPlanner.BLL.Interfaces;
using ETD_Portal.TravelPlanner.DTOs.RequestDTO;
using ETD_Portal.TravelPlanner.DTOs.ResponseDTO;
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

        public TP_ServicesController(
            ILocationServices locationService,
            ITravelRequestServices travelRequestService,
            IBudgetAllocationServices budgetAllocationServices)
        {
            this._locationService = locationService;
            this._travelRequestService = travelRequestService;
            this._budgetAllocationServices = budgetAllocationServices;
        }

        [HttpGet("locations")]
        public async Task<ActionResult<IEnumerable<LocationResponseDTO>>> GetAllLocations()
        {
            try
            {
                var locations = await _locationService.GetAllLocation();
                if (locations == null || !locations.Any())
                    return NotFound("No locations found.");
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("travelrequests/new")]
        public async Task<ActionResult<TravelResponseDTO>> CreateTravelRequest(TravelRequestDTO tRequestDTO)
        {
            try
            {
                var result = await _travelRequestService.CreateTravelRequest(tRequestDTO);
                return StatusCode(201, result);
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

        [HttpGet("travelrequests/{hrId}/pending")]
        public async Task<ActionResult<IEnumerable<TravelResponseDTO>>> GetAllPendingRequests(int hrId)
        {
            try
            {
                var result = await _travelRequestService.GetAllPendingRequests(hrId);
                if (result == null || !result.Any())
                    return NotFound("No pending requests found for this HR.");
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

        [HttpGet("travelrequests/{trid}")]
        public async Task<ActionResult<TravelResponseDTO>> GetTravelRequestById(int trid)
        {
            try
            {
                var result = await _travelRequestService.GetTravelRequestById(trid);
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

        [HttpPut("travelrequests/{trid}/update")]
        public async Task<ActionResult<TravelResponseDTO>> UpdateRequestStatus(int trid, UpdateRequestDTO updateDTO)
        {
            try
            {
                var result = await _travelRequestService.UpdateRequestStatus(trid, updateDTO);
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("travelrequests/calculatebudget")]
        public async Task<ActionResult<int>> CalculateBudget(int travelRequestId)
        {
            try
            {
                var budget = await _budgetAllocationServices.CalculateBudget(travelRequestId);
                return Ok(budget);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
