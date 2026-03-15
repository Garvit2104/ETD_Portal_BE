namespace ETD_Portal.TravelPlanner.DTOs.RequestDTO
{
    public class TravelBudgetAllocationRequestDTO
    {
        public int? travel_request_id { get; set; }
        public int? approved_budget { get; set; }
        public string? approved_mode_of_travel { get; set; }
        public string? approved_hotel_star_rating { get; set; }
    }
}
