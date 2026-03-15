using Reservation_Managmeent_App.DTOs.ReservationTypes_DTO;
using Reservation_Managmeent_App.DAL.ReservationTypes;

namespace Reservation_Managmeent_App.BLL.ReservationTypes
{
    public class ReservationTypesService : IReservationTypeService
    {
        private readonly IReservationTypeRepos _reservationTypeRepos;

        public ReservationTypesService(IReservationTypeRepos _reservationTypeRepos)
        {
            this._reservationTypeRepos = _reservationTypeRepos;
        }
        public async Task<List<ReservationTypeResponseDTO>> GetReservationTypes()
        {
            var resTypes = await _reservationTypeRepos.GetReservationTypes();

            List<ReservationTypeResponseDTO> ls = new List<ReservationTypeResponseDTO>();

            foreach (var item in resTypes)
            {
                ReservationTypeResponseDTO reservationTypeResponse = new ReservationTypeResponseDTO();
                reservationTypeResponse.TypeId = item.TypeId;
                reservationTypeResponse.TypeName = item.TypeName;

                ls.Add(reservationTypeResponse);
            }
            return ls;
        }

    }
}
