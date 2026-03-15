using Reimbursement__Managment.DTOs.ReimbursementType_DTO;
using Reimbursement__Managment.Models;

namespace Reimbursement__Managment.BLL
{
    public interface IReimbursementTypeService
    {
        public Task<IEnumerable<ReimbursementTypeResponseDTO>> GetAllReimbursementType();
    }
}
