namespace ETD_Portal.TravelPlanner.DAL.Interfaces
{
    public interface ILocationRepo
    {
        Task<IEnumerable<Location>> GetAllLocations();
    }
}
