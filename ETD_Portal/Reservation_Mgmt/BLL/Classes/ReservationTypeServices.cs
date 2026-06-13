using AutoMapper;
using ETD_Portal.Reservation_Mgmt.BLL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DAL.Classes;
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
        private readonly IMapper _mapper;
        public ReservationTypeServices(IReservationTypeRepo _reservationTypeRepos, IMapper mapper)
        {
            this._reservationTypeRepos = _reservationTypeRepos;
            this._mapper = mapper;
        }

        public async Task<List<ReservationTypeResponseDTO>> GetReservationTypes()
        {
            var reservationTypes = await _reservationTypeRepos.GetReservationTypes();
            return _mapper.Map<List<ReservationTypeResponseDTO>>(reservationTypes);
        }
    }
}
