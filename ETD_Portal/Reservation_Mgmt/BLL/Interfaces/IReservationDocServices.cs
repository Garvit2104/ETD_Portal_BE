using ETD_Portal.Reservation_Mgmt.DTOs.DownloadDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.BLL.Interfaces
{
    public interface IReservationDocServices
    {
        public Task UploadReservationDocs(int reservationId, IFormFile file);

        public Task<DocDownloadDTO> GetReservationDoc(int reservationId);
    }
}
