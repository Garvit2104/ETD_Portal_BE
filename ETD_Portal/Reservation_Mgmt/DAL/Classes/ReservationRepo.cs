using ETD_Portal.Data;
using ETD_Portal.Models;
using ETD_Portal.Reservation_Mgmt.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.DAL.Classes
{
    public class ReservationRepo : IReservationRepo
    {
        private readonly ETDPortalDbContext _context;
        public ReservationRepo(ETDPortalDbContext _context)
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
            return await _context.Reservations.AsNoTracking().Where(r => r.TravelRequestId == travelRequestId).ToListAsync();

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
            return await _context.Reservations.AsNoTracking().FirstOrDefaultAsync(rid => rid.Id == reservationId);
        }
    }
}
