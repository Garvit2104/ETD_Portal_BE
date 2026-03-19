using ETD_Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.DAL.Interfaces
{
    public interface IReservationTypeRepo
    {
        Task<List<ReservationType>> GetReservationTypes();
    }
}
