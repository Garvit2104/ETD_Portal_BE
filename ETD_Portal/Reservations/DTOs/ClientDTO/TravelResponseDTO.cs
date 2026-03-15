namespace Reservation_Managmeent_App.DTOs.ClientDTO
{
    public class TravelResponseDTO
    {
        public int request_id { get; set; }

        public int? raised_by_employee_id { get; set; }

        public int? to_be_approved_by_hr_id { get; set; }

        public DateTime? request_raised_on { get; set; }

        public DateTime? from_date { get; set; }

        public DateTime? to_date { get; set; }

        public string? purpose_of_travel { get; set; }

        public int? location_id { get; set; }

        public string? request_status { get; set; }

        public DateTime? RequestApprovedOn { get; set; }

        public string? Priority { get; set; }
    }
}
