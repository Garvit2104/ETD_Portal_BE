namespace ETD_Portal.TravelPlanner.DTOs.ResponseDTO
{
    public class TravelResponseDTO
    {
        public int request_id { get; set; }
        public int? raised_by_employee_id { get; set; }
        public int? to_be_approved_by_hr_id { get; set; }
        public DateOnly? from_date { get; set; }
        public DateOnly? to_date { get; set; }
        public string? location_name { get; set; }
        public string? purpose_of_travel { get; set; }
        public string? priority { get; set; }
        public DateOnly? request_raised_on { get; set; }
        public string? request_status { get; set; }
        public DateOnly? request_approved_on { get; set; }
    }
}
