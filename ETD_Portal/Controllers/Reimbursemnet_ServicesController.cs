using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reimbursement__Managment.BLL;
using Reimbursement__Managment.BLL.Reimbursement;
using Reimbursement__Managment.DTOs.Reimbursement;
using Reimbursement__Managment.DTOs.Reimbursement_DTO;

namespace ETD_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Reimbursemnet_ServicesController : ControllerBase
    {
        private readonly IReimbursementTypeService _reimbursementTypeService;
        private readonly IReimbursementService _reimburseService;

        public Reimbursemnet_ServicesController(IReimbursementTypeService _reimbursementTypeService, IReimbursementService _reimburseService)
        {
            this._reimbursementTypeService = _reimbursementTypeService;
            this._reimburseService = _reimburseService;
        }

        [HttpGet("reimbursementTypes/types")]
        public async Task<IActionResult> GetAllReimbursementType()
        {
            var types = await _reimbursementTypeService.GetAllReimbursementType();
            return Ok(types);
        }

        [HttpPost("reimbursement/add")]
        [Consumes("multipart/form-data")]
        public async Task<ReimbursementResponseDTO> AddReimbursement([FromForm] ReimbursementRequestDTO reimburseRequestDTO)
        {
            try
            {
                var result = await _reimburseService.AddReimbursement(reimburseRequestDTO);
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                // Return a generic error response
                throw new Exception($"An error occurred while processing the reimbursement request: {ex.Message}");
            }
        }

        [HttpGet("reimbursements/{travelrequestid}/requests")]

        public async Task<List<ReimbursementResponseDTO>> GetAllReimbursementRequest(int travelrequestid)
        {

        }
    }
}
