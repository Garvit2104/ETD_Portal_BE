using ETD_Portal.TravelPlanner.DTOs.ResponseDTO;

namespace ETD_Portal.TravelPlanner.BLL.Interfaces
{
    public interface ILocationServices
    {
        Task<IEnumerable<LocationResponseDTO>> GetAllLocation();
    }
}
