using Reservation_Managmeent_App.DTOs.ReservationDocs;
using Reservation_Managmeent_App.Models;

namespace Reservation_Managmeent_App.DAL.ReservationDocs
{
    public interface IReservationDocsRepo
    {
        
        public  Task AddReservatonDocs(ReservationDoc docs);

        public Task<ReservationDoc> GetReservationDocByReservationId(int reservationId);
    }
}
