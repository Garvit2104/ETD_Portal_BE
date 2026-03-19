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
        public ReservationTypeRepo(ETDPortalDbContext _context)
        {
            this._context = _context;
        }
        public async Task<List<ReservationType>> GetReservationTypes()
        {

            var result =  await this._context.ReservationTypes.ToListAsync();
            return result;
           
        }
    }
}
