using ETD_Portal.Models;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResponseDTO;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResquestDTO;
using ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.BLL.Interfaces
{
    public interface IReimbursementServices
    {
        public Task<ReimbursementResponseDTO> AddReimbursement(ReimbursementRequestDTO reimburseRequestDTO);

        public Task<List<ReimbursementResponseDTO>> GetAllReimbursementRequest(int trid);

        public Task<ReimbursementResponseDTO> GetReimbursementDetails(int reimbursementid);

        public  Task<ReimbursementResponseDTO> ProcessReimbursemnet(int reimbursementid, ReimbursementRequestDTO reimburseDTO);
    }
}
