namespace ETD_Portal.TravelPlanner.DTOs.ResponseDTO
{
    public class TravelBudgetAllocationResponseDTO
    {
        public int id { get; set; }
        public int? travel_request_id { get; set; }
        public int? approved_budget { get; set; }
        public string? approved_mode_of_travel { get; set; }
        public string? approved_hotel_star_rating { get; set; }
    }
}
