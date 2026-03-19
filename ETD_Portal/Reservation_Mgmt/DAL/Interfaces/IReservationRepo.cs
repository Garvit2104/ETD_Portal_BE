using ETD_Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.DAL.Interfaces
{
    public interface IReservationRepo
    {
        public Task<Reservation> AddReservations(Reservation reservation);

        public Task<List<Reservation>> GetReservationByTravelRequestId(int travelRequestId);

        public Task<int> CountReservationsByTravelRequestId(int travelRequestId);

        public Task<bool> ExistsReservationOfAnyType(int travelRequestId, params int[] typeIds);

        public Task<Reservation> GetReservationDetails(int reservationId);
    }
}
