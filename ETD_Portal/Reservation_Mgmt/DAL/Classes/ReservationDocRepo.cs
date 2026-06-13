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
    public  class ReservationDocRepo : IReservationDocRepo
    {
        private readonly ETDPortalDbContext _context;
        private readonly ILogger<ReservationDocRepo> _logger;
        public ReservationDocRepo(ETDPortalDbContext _context, ILogger<ReservationDocRepo> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        public async Task AddReservatonDocs(ReservationDoc docs)
        {
            await _context.ReservationDocs.AddAsync(docs);
            await _context.SaveChangesAsync();
        }

        public async Task<ReservationDoc> GetReservationDocByReservationId(int reservationId)
        {
            try
            {
                var doc = await _context.ReservationDocs.AsNoTracking().FirstOrDefaultAsync(d => d.ReservationId == reservationId);

                if (doc == null)
                    throw new KeyNotFoundException($"No document record found for ReservationId {reservationId}");

                _logger.LogInformation("GetReservationDocByReservationId: Found document for ReservationId={ReservationId}", reservationId);
                return doc;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetReservationDocByReservationId: Error fetching document for ReservationId={ReservationId}", reservationId);
                throw;
            }
        }
    }
}
