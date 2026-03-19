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
        public ReservationDocRepo(ETDPortalDbContext _context)
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
