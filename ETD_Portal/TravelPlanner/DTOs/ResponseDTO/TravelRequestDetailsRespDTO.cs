using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.TravelPlanner.DTOs.ResponseDTO
{
    public class TravelRequestDetailsRespDTO
    {
        public int request_id { get; set; }
        public DateOnly request_raised_on { get; set; }
        public DateOnly from_date { get; set; }
        public DateOnly to_date { get; set; }
        public string request_status { get; set; }
        public string location_name { get; set; }
        public DateOnly? request_approved_on { get; set; }
        public int? approved_budget { get; set; }
        public string? approved_mode_of_travel { get; set; }
        public string? approved_hotel_star_rating { get; set; }
    }
}
