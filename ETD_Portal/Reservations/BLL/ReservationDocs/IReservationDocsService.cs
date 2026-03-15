using Reservation_Managmeent_App.DTOs.DownloadDTOs;

namespace Reservation_Managmeent_App.BLL.ReservationDocs
{
    public interface IReservationDocsService
    {
        public Task UploadReservationDocs(int reservationId, IFormFile file);

        public Task<DocDownloadDTO> GetReservationDoc(int reservationId);
    }
}
