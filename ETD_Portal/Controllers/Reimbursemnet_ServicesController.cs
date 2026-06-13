using Azure;
using ETD_Portal.Models;
using ETD_Portal.Reimbursement_Mgmt.BLL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResponseDTO;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResquestDTO;
using ETD_Portal.TravelPlanner.DTOs.RequestDTO;
using ETD_Portal.TravelPlanner.DTOs.ResponseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ETD_Portal.Reservation_Mgmt.BLL.Classes.ReservationDocServices;


namespace ETD_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  public class Reimbursemnet_ServicesController : ControllerBase
    {
       private readonly IReimbursementTypeServices _reimbursementTypeService;
       private readonly IReimbursementServices _reimburseService;
        private readonly ILogger<Reimbursemnet_ServicesController> _logger;

        public Reimbursemnet_ServicesController(IReimbursementTypeServices _reimbursementTypeService, 
            IReimbursementServices _reimburseService, ILogger<Reimbursemnet_ServicesController> _logger)
        {
            this._reimbursementTypeService = _reimbursementTypeService;
            this._reimburseService = _reimburseService;
            this._logger = _logger;
        }

        [HttpGet("reimbursementTypes/types")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetAllReimbursementType()
        {
            _logger.LogInformation("Fetching all reimbursement types");
            try
            {
                var types = await _reimbursementTypeService.GetAllReimbursementType();
                return Ok(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching reimbursement types");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost("reimbursement/add")]
        [Authorize(Roles = "Employee")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddReimbursement([FromForm] ReimbursementRequestDTO reimburseRequestDTO)
        {
            _logger.LogInformation(
                "Adding reimbursement request for TravelRequestId {TravelRequestId} by EmployeeId {EmployeeId}",
                reimburseRequestDTO.travel_request_id,
                reimburseRequestDTO.request_raised_by_employee_id);
            try
            {
                var result = await _reimburseService.AddReimbursement(reimburseRequestDTO);
                return Ok(result);
            }
            catch (DocumentSizeLimitExceededException ex)
            {
                _logger.LogWarning("Document size limit exceeded for TravelRequestId {TravelRequestId}: {Reason}",
                    reimburseRequestDTO.travel_request_id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation failed adding reimbursement for TravelRequestId {TravelRequestId}: {Reason}",
                    reimburseRequestDTO.travel_request_id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Resource not found adding reimbursement for TravelRequestId {TravelRequestId}: {Reason}",
                    reimburseRequestDTO.travel_request_id, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding reimbursement for TravelRequestId {TravelRequestId}",
                    reimburseRequestDTO.travel_request_id);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("reimbursements/{travelrequestid}/requests")]
        [Authorize(Roles = "Employee,TravelDeskExe")]
        public async Task<ActionResult<ReimbursementResponseDTO>> GetAllReimbursementRequestbyTrid(int travelrequestid)
        {
            _logger.LogInformation("Fetching all reimbursement requests for TravelRequestId {TravelRequestId}", travelrequestid);
            try
            {
                var response = await _reimburseService.GetAllReimbursementRequest(travelrequestid);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching reimbursement requests for TravelRequestId {TravelRequestId}", travelrequestid);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("reimbursements/{reimbursementid}")]
        [Authorize(Roles = "TravelDeskExe")]

        public async Task<ActionResult<ReimbursementResponseDTO>> GetReimbursementDetails(int reimbursementid)
        {
            _logger.LogInformation("Fetching reimbursement details for ReimbursementId {ReimbursementId}", reimbursementid);
            try
            {
                var response = await _reimburseService.GetReimbursementDetails(reimbursementid);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Reimbursement not found for ReimbursementId {ReimbursementId}: {Reason}", reimbursementid, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching reimbursement details for ReimbursementId {ReimbursementId}", reimbursementid);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPut("reimbursements/{reimbursementid}/update")]
        [Authorize(Roles = "TravelDeskExe")]
        public async Task<IActionResult> UpdateRequestStatus(int reimbursementid, [FromBody] ReimbursementProcessRequestDTO reimburseProcessDTO)
        {
            _logger.LogInformation("Processing reimbursement request for ReimbursementId {ReimbursementId}", reimbursementid);
            try
            {
                var result = await _reimburseService.ProcessReimbursemnet(reimbursementid, reimburseProcessDTO);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation failed processing ReimbursementId {ReimbursementId}: {Reason}", reimbursementid, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Reimbursement not found for ReimbursementId {ReimbursementId}: {Reason}", reimbursementid, ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Invalid operation processing ReimbursementId {ReimbursementId}: {Reason}", reimbursementid, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing ReimbursementId {ReimbursementId}", reimbursementid);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

    }
}
