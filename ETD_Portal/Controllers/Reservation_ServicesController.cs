using ETD_Portal.Reservation_Mgmt.BLL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DTOs.RequestDTO;
using ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ETD_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Reservation_ServicesController : ControllerBase
    {
        private readonly IReservationTypeServices _reservationTypeService;
        private readonly IReservationServices _reservationService;
        private readonly IReservationDocServices _reservationDocService;

        public Reservation_ServicesController(IReservationTypeServices _reservationTypeService, IReservationServices _reservationService, IReservationDocServices _reservationDocService)
        {
            this._reservationTypeService = _reservationTypeService;
            this._reservationService = _reservationService;
            this._reservationDocService = _reservationDocService;
        }


        [HttpGet("types")]

        public async Task<ActionResult<List<ReservationTypeResponseDTO>>> GetReservationType()
        {
            try
            {
                var result = await _reservationTypeService.GetReservationTypes();

                if (result == null || !result.Any())
                    return NotFound("No Reservation found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("add")]
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> AddReservation([FromForm] ReservationRequestDTO addReservation)
        {

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var addedReservation = await _reservationService.AddReservation(addReservation);
            if (addedReservation == null)
                throw new Exception("Reservation record is not found");
            if (addReservation.File is not null && addReservation.File.Length > 0)
            {
                // Use the newly created reservation's Id
                // to link the document to the reservation
                await _reservationDocService.UploadReservationDocs(
                        addedReservation.id,      // ← Id from Step 1
                        addReservation.File);     // ← PDF file
            }
            return Ok(addedReservation);
        }

        [HttpGet("track/{travelRequestid}")]

        public async Task<IActionResult> TrackReservationsByTravelRequestId(int travelRequestid)
        {
            var response = await _reservationService.GetReservationByTravelRequestId(travelRequestid);

            if (response == null || response.Count == 0)
                return NotFound(new
                {
                    message =
                    $"No reservations found for TravelRequestId: {travelRequestid}"
                });

            return Ok(response);
        }

        [HttpGet("reservationid")]

        public async Task<IActionResult> GetReservationDetails(int reservationid)
        {
            var response = await _reservationService.GetReservationDetails(reservationid);

            if (response == null)
                return NotFound(new
                {
                    message =
                    $"Reservation {reservationid} not found"
                });
            return Ok(response);
        }

        [HttpGet("{reservationId}/download")]
        public async Task<IActionResult> DownloadReservationDoc(int reservationId)
        {
            var fileResult = await _reservationDocService.GetReservationDoc(reservationId);
            if (fileResult == null)
            {
                return NotFound("Document Not found");
            }
            return File(fileResult.FileBytes, "application/pdf", fileResult.FileName);
        }
    }
}
