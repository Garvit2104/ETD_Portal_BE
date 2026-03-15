using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reservation_Managmeent_App.BLL.ReservationDocs;
using Reservation_Managmeent_App.BLL.Reservations;
using Reservation_Managmeent_App.BLL.ReservationTypes;
using Reservation_Managmeent_App.DTOs.ReservationsDTO;

namespace ETD_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Reservation_ServicesController : ControllerBase
    {
        private readonly IReservationTypeService _reservationTypeService;
        private readonly IReservationService _reservationService;
        private readonly IReservationDocsService _reservationDocsService;
        private readonly IReservationDocsService _reservationDocsServices;

        public Reservation_ServicesController(IReservationTypeService _reservationTypeService, IReservationService _reservationService, IReservationDocsService _reservationDocsService, IReservationDocsService reservationDocsServices)
        {
            this._reservationTypeService = _reservationTypeService;
            this._reservationService = _reservationService;
            this._reservationDocsService = _reservationDocsService;
            _reservationDocsServices = reservationDocsServices;
            this._reservationDocsServices = _reservationDocsServices;
        }

        [HttpGet("types")]

        public async Task<IActionResult> GetReservationType()
        {
            var result = await _reservationTypeService.GetReservationTypes();
            return Ok(result);
        }
        [HttpPost("add")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddReservation([FromForm] ReservationRequestDTO addReservation)
        {
            var addedReservation = await _reservationService.AddReservation(addReservation);
            if (addReservation.File != null)
            {
                // Use the newly created reservation's Id
                // to link the document to the reservation
                await _reservationDocsService
                    .UploadReservationDocs(
                        addedReservation.Id,      // ← Id from Step 1
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
            var fileResult = await _reservationDocsServices.GetReservationDoc(reservationId);
            if (fileResult == null)
            {
                return NotFound("Document Not found");
            }
            return File(fileResult.FileBytes, "application/pdf", fileResult.FileName);
        }
    }
}
