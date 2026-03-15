    using Reservation_Managmeent_App.DTOs.Reservations;
    using Reservation_Managmeent_App.DTOs.ReservationsDTO;

    namespace Reservation_Managmeent_App.BLL.Reservations
    {
        public interface IReservationService
        {
            Task<ReservationResponseDTO> AddReservation(ReservationRequestDTO addReservationRecord);

            public Task<List<ReservationResponseDTO>> GetReservationByTravelRequestId(int trid);


            public Task<ReservationResponseDTO> GetReservationDetails(int reservationId);
        }
    }
