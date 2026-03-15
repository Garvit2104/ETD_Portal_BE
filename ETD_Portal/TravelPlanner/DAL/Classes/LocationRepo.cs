using ETD_Portal.TravelPlanner.DAL.Interfaces;

namespace ETD_Portal.TravelPlanner.DAL.Classes
{
    public class LocationRepo : ILocationRepo
    {
        private readonly ETDPortalDbContext _context;

        public LocationRepo(ETDPortalDbContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<Location>> GetAllLocations()
        {
            return await _context.Locations.AsNoTracking().ToListAsync();
        }
    }
}
