using ETD_Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.DAL.Interfaces
{
    public interface IReimbursementTypeRepo
    {
        public Task<List<ReimbursementType>> GetAllReimbursementType();

        public Task<ReimbursementType> GetTypeById(int id);
    }
}
