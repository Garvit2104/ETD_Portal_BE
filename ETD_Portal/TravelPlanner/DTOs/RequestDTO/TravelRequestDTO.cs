namespace ETD_Portal.TravelPlanner.DTOs.RequestDTO
{
    public class TravelRequestDTO
    {
        public int raised_by_employee_id { get; set; }
        public int to_be_approved_by_hr_id { get; set; }
        public DateOnly? from_date { get; set; }
        public DateOnly? to_date { get; set; }
        public int? location_id { get; set; }
        public string? purpose_of_travel { get; set; }
        public string? priority { get; set; }
    }
}
