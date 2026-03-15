using Microsoft.AspNetCore.Mvc;
using Reimbursement__Managment.DTOs.Reimbursement;
using Reimbursement__Managment.DTOs.Reimbursement_DTO;
using Reimbursement__Managment.Models;

namespace Reimbursement__Managment.DAL.Reimbursements
{
    public interface IReimbursementRepo
    {

        public Task<ReimbursementRequest> AddReimbursement(ReimbursementRequest entity);

        public Task<IEnumerable<ReimbursementRequest>> GetAllReimbursementRequest(int travelrequestid);
    }
}
