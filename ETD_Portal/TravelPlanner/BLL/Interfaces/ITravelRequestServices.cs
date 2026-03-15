using ETD_Portal.TravelPlanner.DTOs.RequestDTO;
using ETD_Portal.TravelPlanner.DTOs.ResponseDTO;

namespace ETD_Portal.TravelPlanner.BLL.Interfaces
{
    public class ITravelRequestServices
    {
        Task<TravelResponseDTO> CreateTravelRequest(TravelRequestDTO travelRequestDTO);
        Task<IEnumerable<TravelResponseDTO>> GetAllPendingRequests(int hrId);
        Task<TravelResponseDTO> GetTravelRequestById(int trid);
        Task<TravelResponseDTO> UpdateRequestStatus(int trid, UpdateRequestDTO updateDTO);
    }
}
