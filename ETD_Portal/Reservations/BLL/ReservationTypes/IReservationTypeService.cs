using Reservation_Managmeent_App.DTOs.ReservationTypes_DTO;

namespace Reservation_Managmeent_App.BLL.ReservationTypes
{
    public interface IReservationTypeService
    {
        Task<List<ReservationTypeResponseDTO>> GetReservationTypes();
    }
}
