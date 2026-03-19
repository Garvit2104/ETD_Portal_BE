using ETD_Portal.Reservation_Mgmt.BLL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DAL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.BLL.Classes
{
    public class ReservationTypeServices : IReservationTypeServices
    {
        private readonly IReservationTypeRepo _reservationTypeRepos;
        public ReservationTypeServices(IReservationTypeRepo _reservationTypeRepos)
        {
            this._reservationTypeRepos = _reservationTypeRepos;
        }

        public async Task<List<ReservationTypeResponseDTO>> GetReservationTypes()
        {
            var resTypes = await _reservationTypeRepos.GetReservationTypes();

            List<ReservationTypeResponseDTO> reservationlist = new List<ReservationTypeResponseDTO>();

            foreach (var item in resTypes)
            {
                ReservationTypeResponseDTO reservationTypeResponse = new ReservationTypeResponseDTO();
                reservationTypeResponse.type_id = item.TypeId;
                reservationTypeResponse.type_name = item.TypeName;

                reservationlist.Add(reservationTypeResponse);
            }
            return reservationlist;
        }
    }
}
