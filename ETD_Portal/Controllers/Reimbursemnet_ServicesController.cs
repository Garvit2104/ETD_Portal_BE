using Azure;
using ETD_Portal.Models;
using ETD_Portal.Reimbursement_Mgmt.BLL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResponseDTO;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResquestDTO;
using ETD_Portal.TravelPlanner.DTOs.RequestDTO;
using ETD_Portal.TravelPlanner.DTOs.ResponseDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ETD_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  public class Reimbursemnet_ServicesController : ControllerBase
    {
       private readonly IReimbursementTypeServices _reimbursementTypeService;
       private readonly IReimbursementServices _reimburseService;

        public Reimbursemnet_ServicesController(IReimbursementTypeServices _reimbursementTypeService, IReimbursementServices _reimburseService)
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
                throw new Exception($"An error occurred while processing the reimbursement request: {ex.Message}");
            }
        }

       [HttpGet("reimbursements/{travelrequestid}/requests")]

        public async Task<ActionResult<ReimbursementResponseDTO>> GetAllReimbursementRequestbyTrid(int travelrequestid)
        {
            try
            {
                var response = await _reimburseService.GetAllReimbursementRequest(travelrequestid);

                if (response == null || response.Count == 0)
                    return NotFound($"No Reimbursement Found with: {travelrequestid} ");
                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("reimbursements/{reimbursementid}")]

        public async Task<ActionResult<ReimbursementResponseDTO>> GetReimbursementDetails(int travelrequestid)
        {
            try
            {
                var response = await _reimburseService.GetReimbursementDetails(travelrequestid);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("reimbursements/{reimbursementid}/update")]
        public async Task<ActionResult<TravelResponseDTO>> UpdateRequestStatus(int reimbursementid, ReimbursementRequestDTO requestDTO)
        {
            try
            {
                var result = await _reimburseService.ProcessReimbursemnet(reimbursementid, requestDTO);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
