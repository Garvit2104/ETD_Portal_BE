using Reimbursement__Managment.BLL;
using Reimbursement__Managment.DAL;
using Reimbursement__Managment.DTOs.ReimbursementType_DTO;
namespace Reimbursement__Managment.BLL
{
    public class ReimbursementTypeServiceClass : IReimbursementTypeService
    {
        private readonly IReimbursementTypeRepo _typesRepo;
        public ReimbursementTypeServiceClass(IReimbursementTypeRepo _typesRepo)
        {
            this._typesRepo = _typesRepo;
        }
        public async Task<IEnumerable<ReimbursementTypeResponseDTO>> GetAllReimbursementType()
        {
            var types = await _typesRepo.GetAllReimbursementType();
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
