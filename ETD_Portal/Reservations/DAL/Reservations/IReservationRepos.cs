using Reservation_Managmeent_App.Models;

namespace Reservation_Managmeent_App.DAL.Reservations
{
    public interface IReservationRepos
    {
        public Task<Reservation> AddReservations(Reservation reservation);

        public Task<List<Reservation>> GetReservationByTravelRequestId(int travelRequestId);

        public Task<int> CountReservationsByTravelRequestId(int travelRequestId);


        public Task<bool> ExistsReservationOfAnyType(int travelRequestId, params int[] typeIds);

        public Task<Reservation> GetReservationDetails(int reservationId);
    }
}
