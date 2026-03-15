using Reservation_Managmeent_App.Models;

namespace Reservation_Managmeent_App.DTOs.ReservationDocs
{
    public class ReservationDocsResponseDTO
    {
        public int Id { get; set; }

        public int? ReservationId { get; set; }

        public string? DocumentUrl { get; set; }

        
    }
}
