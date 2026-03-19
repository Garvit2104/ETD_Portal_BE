using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.DTOs.RequestDTO
{
    public class ReservationDocRequestDTO
    {
        public int id { get; set; }

        public int? reservation_id { get; set; }

        public string? document_url { get; set; }
    }
}
