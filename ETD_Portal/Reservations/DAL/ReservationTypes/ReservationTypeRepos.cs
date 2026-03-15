using Microsoft.EntityFrameworkCore;
using Reservation_Managmeent_App.Data;
using Reservation_Managmeent_App.Models;

namespace Reservation_Managmeent_App.DAL.ReservationTypes
{
    public class ReservationTypeRepos : IReservationTypeRepos
    {
        private readonly ReservationDbContext _context;

        public ReservationTypeRepos(ReservationDbContext _context)
        {
            this._context = _context;
        }

        public async Task<List<ReservationType>> GetReservationTypes()
        {
            // ✅ ToListAsync() is the async version of ToList()
            // await means: "wait for database to respond, but don't block the thread"
            // WITHOUT await: thread is frozen until DB responds
            // WITH await:    thread goes off to handle other requests, comes back when DB is done
            var data = await _context.ReservationTypes.ToListAsync();
            return data;
        }
    }
}
