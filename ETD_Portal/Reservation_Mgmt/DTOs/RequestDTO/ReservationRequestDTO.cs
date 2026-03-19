using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.DTOs.RequestDTO
{
    public class ReservationRequestDTO
    {
        public int? reservation_done_by_employee_id { get; set; }

        public int? travel_request_id { get; set; }

        public int? reservation_type_id { get; set; }

        public DateOnly? created_on { get; set; }

        public string? reservation_done_with_entity { get; set; }

        public DateOnly? reservation_date { get; set; }

        public int? amount { get; set; }

        //public string? confirmation_id { get; set; }

        public string? remarks { get; set; }

        public IFormFile? File { get; set; }


    }
}
