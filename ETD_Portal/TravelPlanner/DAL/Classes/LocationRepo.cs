using ETD_Portal.Data;
using ETD_Portal.Models;
using ETD_Portal.TravelPlanner.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.TravelPlanner.DAL.Classes
{
    public class LocationRepo : ILocationRepo
    {
        private readonly ETDPortalDbContext _context;
        private readonly ILogger<LocationRepo> _logger;
        public LocationRepo(ETDPortalDbContext context, ILogger<LocationRepo> logger)
        {
            this._context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Location>> GetAllLocations()
        {
            try
            {
            return await _context.Locations.AsNoTracking().ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in GetAllLocations");
                throw;
            }
        }
    }
}
