using ETD_Portal.TravelPlanner.BLL.Interfaces;
using ETD_Portal.TravelPlanner.DAL.Interfaces;
using ETD_Portal.TravelPlanner.DTOs.ResponseDTO;

namespace ETD_Portal.TravelPlanner.BLL.Classes
{
    public class LocationServices : ILocationServices
    {

        private readonly ILocationRepo _locationRepo;

        public LocationServices(ILocationRepo locationRepo)
        {
            this._locationRepo = locationRepo;
        }

        public async Task<IEnumerable<LocationResponseDTO>> GetAllLocation()
        {
            var locations = await _locationRepo.GetAllLocations();
            return locations.Select(l => new LocationResponseDTO
            {
                id = l.Id,
                name = l.Name
            });
        }
    }
}
