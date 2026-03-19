using ETD_Portal.Data;
using ETD_Portal.Models;
using ETD_Portal.Reimbursement_Mgmt.BLL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.DAL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResponseDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.BLL.Classes
{
    public class ReimbursementTypeServices : IReimbursementTypeServices
    {
        private readonly IReimbursementTypeRepo _reimbursementTypesRepo;
        public ReimbursementTypeServices(IReimbursementTypeRepo _reimbursementTypesRepo)
        {
            this._reimbursementTypesRepo = _reimbursementTypesRepo;
        }

        public async Task<List<ReimbursementTypeResponseDTO>> GetAllReimbursementType()
        {
            var types = await _reimbursementTypesRepo.GetAllReimbursementType();
            List<ReimbursementTypeResponseDTO> reimbursetype = new List<ReimbursementTypeResponseDTO>();

            foreach (var item in types)
            {
                ReimbursementTypeResponseDTO responseDTO = new ReimbursementTypeResponseDTO();

                responseDTO.id = item.Id;
                responseDTO.type = item.Type;
                reimbursetype.Add(responseDTO);
            }
            return reimbursetype;
        }

    }
}
