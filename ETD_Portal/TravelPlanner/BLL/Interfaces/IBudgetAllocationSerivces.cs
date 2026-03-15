namespace ETD_Portal.TravelPlanner.BLL.Interfaces
{
    public interface IBudgetAllocationSerivces
    {
        Task<int> CalculateApprovedBudget(int? employeeId, string? priority, int days);
        Task<string> CalculateHotelStarRating(int? employeeId);
        Task<string> CalculateModeOfTravel();
        Task AddBudgetAllocation(TravelRequest approvedRequest);
        Task<int> CalculateBudget(int travelRequestId);
    }
}
