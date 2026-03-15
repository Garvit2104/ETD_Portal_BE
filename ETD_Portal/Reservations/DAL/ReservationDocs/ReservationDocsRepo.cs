using Microsoft.EntityFrameworkCore;
using Reservation_Managmeent_App.Data;
using Reservation_Managmeent_App.DTOs.ReservationDocs;
using Reservation_Managmeent_App.Models;

namespace Reservation_Managmeent_App.DAL.ReservationDocs
{
    public class ReservationDocsRepo : IReservationDocsRepo
    {
        private readonly ReservationDbContext _context;
        public ReservationDocsRepo(ReservationDbContext _context)
        {
            this._context = _context;
        }

        public async Task AddReservatonDocs(ReservationDoc docs)
        {
            await _context.ReservationDocs.AddAsync(docs);
            await _context.SaveChangesAsync();
        }

        public async Task<ReservationDoc> GetReservationDocByReservationId(int reservationId) 
        { 
            return await _context.ReservationDocs.AsNoTracking().FirstOrDefaultAsync(d => d.ReservationId == reservationId); 
        }
    }
}
