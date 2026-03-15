using Microsoft.AspNetCore.Mvc;
using Reimbursement__Managment.DTOs.Reimbursement;
using Reimbursement__Managment.DTOs.Reimbursement_DTO;
using Reimbursement__Managment.Models;

namespace Reimbursement__Managment.BLL.Reimbursement
{
    public interface IReimbursementService
    {
        public Task<ReimbursementResponseDTO> AddReimbursement([FromForm] ReimbursementRequestDTO reimburseRequestDTO);

        public Task<IEnumerable<ReimbursementRequest>> GetAllReimbursementRequest(int travelrequestid);
    }
}
