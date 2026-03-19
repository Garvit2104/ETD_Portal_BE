using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO
{
    public class ReservationDocResponseDTO
    {
        public int? reservation_id { get; set; }

        public string? document_url { get; set; }
    }
}
