namespace ETD_Portal.TravelPlanner.DAL.Interfaces
{
    public interface ITravelRequestRepo
    {
        Task<TravelRequest> CreateTravelRequest(TravelRequest travelRequest);
        Task<IEnumerable<TravelRequest>> GetAllPendingRequests(int hrId);
        Task<TravelRequest> getTravelRequestById(int trid);
        Task<TravelRequest> getUpdateRequestStatus(TravelRequest travelRequest);
    }
}
