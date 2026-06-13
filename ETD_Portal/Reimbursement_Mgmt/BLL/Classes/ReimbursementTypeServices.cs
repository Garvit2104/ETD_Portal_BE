using AutoMapper;
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
        private readonly IMapper _mapper;
        public ReimbursementTypeServices(IReimbursementTypeRepo _reimbursementTypesRepo, IMapper _mapper)
        {
            this._reimbursementTypesRepo = _reimbursementTypesRepo;
            this._mapper = _mapper;
        }

        public async Task<List<ReimbursementTypeResponseDTO>> GetAllReimbursementType()
        {
            var types = await _reimbursementTypesRepo.GetAllReimbursementType();

            return _mapper.Map<List<ReimbursementTypeResponseDTO>>(types);
        }

    }
}
