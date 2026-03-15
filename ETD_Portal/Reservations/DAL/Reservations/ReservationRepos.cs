using Microsoft.EntityFrameworkCore;
using Reservation_Managmeent_App.Data;
using Reservation_Managmeent_App.Models;
namespace Reservation_Managmeent_App.DAL.Reservations
{
    public class ReservationRepos: IReservationRepos
    {
        private readonly ReservationDbContext _context;

        public ReservationRepos(ReservationDbContext _context)
        {
            this._context = _context;
        }

        public async Task<Reservation> AddReservations(Reservation reservation)
        {
            var addedReservation = await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
            return addedReservation.Entity;
        }

        public async Task<List<Reservation>> GetReservationByTravelRequestId(int travelRequestId)
        {
            return  await _context.Reservations.AsNoTracking().Where(r=>r.TravelRequestId == travelRequestId).ToListAsync();
            
        }


        public async Task<int> CountReservationsByTravelRequestId(int travelRequestId)
        {
            return await _context.Reservations
                .AsNoTracking()
                .CountAsync(r => r.TravelRequestId == travelRequestId);
        }

        public async Task<bool> ExistsReservationOfAnyType(int travelRequestId, params int[] typeIds)
        {
            return await _context.Reservations
                           .AsNoTracking()
                           .AnyAsync(r => r.TravelRequestId == travelRequestId
                                  && r.ReservationTypeId.HasValue
                                  && typeIds.Contains(r.ReservationTypeId.Value));
        }

        public async Task<Reservation> GetReservationDetails(int reservationId)
        {
            return await _context.Reservations.AsNoTracking().FirstOrDefaultAsync(rid => rid.ReservationDoneByEmployeeId == reservationId);
        }

    }
}
