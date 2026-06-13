using ETD_Portal.Reservation_Mgmt.BLL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DTOs.RequestDTO;
using ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ETD_Portal.Reservation_Mgmt.BLL.Classes.ReservationDocServices;


namespace ETD_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Reservation_ServicesController : ControllerBase
    {
        private readonly IReservationTypeServices _reservationTypeService;
        private readonly IReservationServices _reservationService;
        private readonly IReservationDocServices _reservationDocService;
        private readonly ILogger<Reservation_ServicesController> _logger;

        public Reservation_ServicesController(IReservationTypeServices _reservationTypeService, 
            IReservationServices _reservationService, 
            IReservationDocServices _reservationDocService, ILogger<Reservation_ServicesController> logger)
        {
            this._reservationTypeService = _reservationTypeService;
            this._reservationService = _reservationService;
            this._reservationDocService = _reservationDocService;
            this._logger = logger;
        }


        [HttpGet("types")]
        [Authorize(Roles = "TravelDeskExe")]
        public async Task<ActionResult<List<ReservationTypeResponseDTO>>> GetReservationType()
        {
            _logger.LogInformation("GetReservationType: Retrieving all reservation types");
            try
            {
                var result = await _reservationTypeService.GetReservationTypes();

                    if (result == null || !result.Any())
                    {
                        _logger.LogWarning("GetReservationType: No reservation types found in database");
                        return Ok(new List<ReservationTypeResponseDTO>());
                    }


                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetReservationType: Unexpected error occurred while retrieving reservation types");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpPost("add")]
        [Authorize(Roles = "TravelDeskExe")]
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> AddReservation([FromForm] ReservationRequestDTO addReservation)
        {
            _logger.LogInformation("AddReservation: Adding reservation for TravelRequestId={TravelRequestId}, ReservationTypeId={ReservationTypeId}",
            addReservation.travel_request_id, addReservation.reservation_type_id);

            try
            {
                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);
                var addedReservation = await _reservationService.AddReservation(addReservation);

                if (addReservation.File is not null && addReservation.File.Length > 0)
                {
                    await _reservationDocService.UploadReservationDocs(
                        addedReservation.id,
                        addReservation.File);
                }

                return Ok(addedReservation);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning("AddReservation: Null payload — {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("AddReservation: Validation failed — {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("AddReservation: Business rule violation — {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("AddReservation: Unauthorized — {Message}", ex.Message);
                return StatusCode(403, new { message = ex.Message });
            }
            catch (DocumentSizeLimitExceededException ex)
            {
                _logger.LogWarning("AddReservation: Document size exceeded — {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddReservation: Unexpected error occurred while adding reservation for TravelRequestId={TravelRequestId}", addReservation.travel_request_id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpGet("track/{travelRequestid}")]
        [Authorize(Roles = "Employee")]

        public async Task<IActionResult> TrackReservationsByTravelRequestId(int travelRequestid)
        {
            _logger.LogInformation("TrackReservationsByTravelRequestId: Fetching reservations for TravelRequestId={TravelRequestId}", travelRequestid);
            try
            {
                var response = await _reservationService.GetReservationByTravelRequestId(travelRequestid);

                if (response == null || response.Count == 0)
                    
                    {
                        _logger.LogWarning("TrackReservationsByTravelRequestId: No reservations found for TravelRequestId={TravelRequestId}", travelRequestid);
                        return Ok(new List<ReservationResponseDTO>());
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TrackReservationsByTravelRequestId: Unexpected error for TravelRequestId={TravelRequestId}", travelRequestid);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("{reservationid}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetReservationDetails(int reservationid)
        {
            _logger.LogInformation("GetReservationDetails: Fetching reservation for ReservationId={ReservationId}", reservationid);
            try
            {
                var response = await _reservationService.GetReservationDetails(reservationid);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("GetReservationDetails: Reservation not found for ReservationId={ReservationId}", reservationid);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetReservationDetails: Unexpected error for ReservationId={ReservationId}", reservationid);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("budget/{travelRequestId}")]
        [Authorize(Roles = "TravelDeskExe")]
        public async Task<IActionResult> GetBudgetBreakdown(int travelRequestId)
        {
            _logger.LogInformation("GetBudgetBreakdown: Fetching budget breakdown for TravelRequestId={TravelRequestId}", travelRequestId);
            try
            {
                var result = await _reservationService.GetBudgetBreakdown(travelRequestId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("GetBudgetBreakdown: Travel request not found for TravelRequestId={TravelRequestId}", travelRequestId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBudgetBreakdown: Unexpected error for TravelRequestId={TravelRequestId}", travelRequestId);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }


        [HttpGet("{reservationId}/download")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DownloadReservationDoc(int reservationId)
        {
            _logger.LogInformation("DownloadReservationDoc: Download requested for ReservationId={ReservationId}", reservationId);
            try
            {
                var fileResult = await _reservationDocService.GetReservationDoc(reservationId);
                return File(fileResult.FileBytes, "application/pdf", fileResult.FileName);

            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("DownloadReservationDoc: Document not found for ReservationId={ReservationId}", reservationId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DownloadReservationDoc: Unexpected error for ReservationId={ReservationId}", reservationId);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}
