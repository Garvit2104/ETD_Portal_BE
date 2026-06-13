using ETD_Portal.TravelPlanner.DTOs.RequestDTO;
using ETD_Portal.TravelPlanner.DTOs.ResponseDTO;

namespace ETD_Portal.TravelPlanner.BLL.Interfaces
{
    public interface ITravelRequestServices
    {
        public Task<TravelResponseDTO> CreateTravelRequest(TravelRequestDTO travelRequestDTO);
        public Task<IEnumerable<TravelResponseDTO>> GetAllPendingRequests(int hrId);
        public Task<TravelResponseDTO> GetTravelRequestById(int? trid);

        Task<TravelRequestDetailsRespDTO> GetTravelRequestDetailsById(int trid);
        public Task<TravelRequestDetailsRespDTO> UpdateRequestStatus(int trid, UpdateRequestDTO updateDTO);
    }
}
