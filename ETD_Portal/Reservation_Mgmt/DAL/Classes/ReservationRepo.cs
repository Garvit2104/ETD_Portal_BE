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
        private readonly ILogger<ReservationRepo> _logger;
        public ReservationRepo(ETDPortalDbContext _context, ILogger<ReservationRepo> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }
        public async Task<Reservation> AddReservations(Reservation reservation)
        {
            try
            {
                var addedReservation = await _context.Reservations.AddAsync(reservation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("AddReservations: Saved reservation with Id={ReservationId} for TravelRequestId={TravelRequestId}",
                    addedReservation.Entity.Id, addedReservation.Entity.TravelRequestId);
                return addedReservation.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddReservations: Error saving reservation for TravelRequestId={TravelRequestId}", reservation.TravelRequestId);
                throw;
            }
        }

        public async Task<List<Reservation>> GetReservationByTravelRequestId(int travelRequestId)
        {
            try
            {
                var reservations = await _context.Reservations.AsNoTracking().Where(r => r.TravelRequestId == travelRequestId).ToListAsync();
                _logger.LogInformation("GetReservationByTravelRequestId: Fetched {Count} reservations for TravelRequestId={TravelRequestId}", reservations.Count, travelRequestId);
                return reservations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetReservationByTravelRequestId: Error fetching reservations for TravelRequestId={TravelRequestId}", travelRequestId);
                throw;
            }
        }

        public async Task<int> CountReservationsByTravelRequestId(int travelRequestId)
        {
            try
            {
                return await _context.Reservations.AsNoTracking().CountAsync(r => r.TravelRequestId == travelRequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CountReservationsByTravelRequestId: Error counting reservations for TravelRequestId={TravelRequestId}", travelRequestId);
                throw;
            }
        }

        public async Task<bool> ExistsReservationOfAnyType(int travelRequestId, params int[] typeIds)
        {
            try
            {
                return await _context.Reservations
                    .AsNoTracking()
                    .AnyAsync(r => r.TravelRequestId == travelRequestId
                           && r.ReservationTypeId.HasValue
                           && typeIds.Contains(r.ReservationTypeId.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExistsReservationOfAnyType: Error checking reservation types for TravelRequestId={TravelRequestId}", travelRequestId);
                throw;
            }
        }


        public async Task<Reservation> GetReservationDetails(int reservationId)
        {
            try
            {
                var reservation = await _context.Reservations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == reservationId);
                if (reservation == null)
                    throw new KeyNotFoundException($"Reservation with Id {reservationId} not found");

                _logger.LogInformation("GetReservationDetails: Found reservation for ReservationId={ReservationId}", reservationId);
                return reservation;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetReservationDetails: Error fetching reservation for ReservationId={ReservationId}", reservationId);
                throw;
            }
        }
    }
}
