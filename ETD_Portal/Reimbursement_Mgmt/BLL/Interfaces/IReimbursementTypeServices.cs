using ETD_Portal.Models;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.BLL.Interfaces
{
    public interface IReimbursementTypeServices
    {
        public Task<List<ReimbursementTypeResponseDTO>> GetAllReimbursementType();
    }
}
