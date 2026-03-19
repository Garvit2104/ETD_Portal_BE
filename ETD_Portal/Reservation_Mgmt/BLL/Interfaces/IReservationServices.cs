using ETD_Portal.Reservation_Mgmt.DTOs.RequestDTO;
using ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.BLL.Interfaces
{
    public interface IReservationServices
    {
        Task<ReservationResponseDTO> AddReservation(ReservationRequestDTO addReservationRecord);

        public Task<List<ReservationResponseDTO>> GetReservationByTravelRequestId(int trid);


        public Task<ReservationResponseDTO> GetReservationDetails(int reservationId);
    }
}
