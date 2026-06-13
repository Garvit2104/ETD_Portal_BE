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
    public class ReservationTypeRepo : IReservationTypeRepo
    {
        private readonly ETDPortalDbContext _context;
        private readonly ILogger<ReservationTypeRepo> _logger;
        public ReservationTypeRepo(ETDPortalDbContext _context, ILogger<ReservationTypeRepo> logger)
        {
            this._context = _context;
            this._logger = logger;
        }
        public async Task<List<ReservationType>> GetReservationTypes()
        {
            try
            {
                var reservationTypes = await _context.ReservationTypes.ToListAsync();
                _logger.LogInformation("GetReservationTypes: Fetched {Count} records", reservationTypes.Count);
                return reservationTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetReservationTypes: Error occurred while fetching reservation types from database");
                throw;
            }

        }
    }
}
