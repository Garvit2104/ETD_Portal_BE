using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO
{
    public class ReservationTypeResponseDTO
    {
        public int type_id { get; set; }

        public string? type_name { get; set; }
    }
}
