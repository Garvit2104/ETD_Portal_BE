using Reservation_Managmeent_App.Models;

namespace Reservation_Managmeent_App.DAL.ReservationTypes
{
    public interface IReservationTypeRepos
    {
        Task<List<ReservationType>> GetReservationTypes();
    }
}
