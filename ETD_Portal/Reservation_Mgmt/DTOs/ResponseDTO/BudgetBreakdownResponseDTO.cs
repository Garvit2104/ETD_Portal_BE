using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO
{
    public class BudgetBreakdownResponseDTO
    {
        public int travel_request_id { get; set; }
        public int approved_budget { get; set; }
        public int max_reservation_budget { get; set; }
        public int max_transport { get; set; }
        public int max_hotel { get; set; }
        public int max_cab { get; set; }
    }
}
