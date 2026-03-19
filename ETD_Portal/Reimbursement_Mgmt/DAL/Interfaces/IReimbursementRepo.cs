using ETD_Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.DAL.Interfaces
{
    public interface IReimbursementRepo
    {
        public Task<ReimbursementRequest> AddReimbursement(ReimbursementRequest entity);

        public Task<List<ReimbursementRequest>> GetAllReimbursementRequest(int trid);

        public Task<ReimbursementRequest> GetReimbursementDetails(int reimbursementid);

        public  Task<ReimbursementRequest> ProcessReimbursemnet(ReimbursementRequest reimbursementRequest);
    }
}
