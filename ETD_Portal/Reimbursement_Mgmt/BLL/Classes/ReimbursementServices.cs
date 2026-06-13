using AutoMapper;
using ETD_Portal.HR_Management.BLL.Classes;
using ETD_Portal.HR_Management.BLL.Interfaces;
using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.Models;
using ETD_Portal.Reimbursement_Mgmt.BLL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.DAL.Interfaces;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResponseDTO;
using ETD_Portal.Reimbursement_Mgmt.DTOs.ResquestDTO;
using ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO;
using ETD_Portal.TravelPlanner.BLL.Interfaces;
using ETD_Portal.TravelPlanner.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ETD_Portal.Reservation_Mgmt.BLL.Classes.ReservationDocServices;

namespace ETD_Portal.Reimbursement_Mgmt.BLL.Classes
{
    public class ReimbursementServices : IReimbursementServices
    {
        private readonly IReimbursementRepo _reimbursementRepo;
        private readonly IUserServices _userServices;
        private readonly ITravelRequestServices _travelRequestServices;
        private readonly IReimbursementTypeRepo _reimbursementTypeRepo;
        private readonly IMapper _mapper;
        public ReimbursementServices(IReimbursementRepo _reimbursementRepo,IMapper _mapper,
                                     IUserServices _userServices, ITravelRequestServices _travelRequestServices, 
                                     IReimbursementTypeRepo _reimbursementTypeRepo)
        {
            this._reimbursementRepo = _reimbursementRepo;
            this._userServices = _userServices;
            this._travelRequestServices = _travelRequestServices;   
            this._reimbursementTypeRepo = _reimbursementTypeRepo;
            this._mapper = _mapper;
            
        }

        public async Task<ReimbursementResponseDTO> AddReimbursement(ReimbursementRequestDTO reimburseRequestDTO)
        {
            // step 1 -> Validate the file
            if (reimburseRequestDTO.document == null || reimburseRequestDTO.document.Length == 0)
            {
                throw new ArgumentException("File is required");
            }
            if (!reimburseRequestDTO.document.ContentType.Equals("application/pdf"))
            {
                throw new ArgumentException("Only pdf documents are allowed");
            }
            if (reimburseRequestDTO.document.Length > 256 * 1024)
            {
                throw new DocumentSizeLimitExceededException("Document Size must not exceed 256 kb");
            }
            // step 2 -> Valide employee_id

            var employee = await _userServices.GetEmployeeById(reimburseRequestDTO.request_raised_by_employee_id);
            if (employee == null)
            {
                throw new KeyNotFoundException(
                    $"Employee with ID {reimburseRequestDTO.request_raised_by_employee_id} not found.");
            }

            // step 3 -> Validate Invoice Details
            var travelDetails = await _travelRequestServices.GetTravelRequestById(reimburseRequestDTO.travel_request_id);
            if (travelDetails == null)
            {
                throw new KeyNotFoundException(
                      $"Travel request with ID {reimburseRequestDTO.travel_request_id} not found.");
            }
            DateOnly fromDate = travelDetails.from_date.Value;
            DateOnly toDate = travelDetails.to_date.Value;

            DateOnly invoiceDate = reimburseRequestDTO.invoice_date.Value;
            var validInvoiceDate = fromDate < invoiceDate && invoiceDate < toDate;

            if (!validInvoiceDate)
            {
                throw new ArgumentException("Invoice date must be between from date and to date of travel request");
            }


            //  Per Day Expense Validation for Local Travel
            var reimbursementType = await _reimbursementTypeRepo.GetTypeById(reimburseRequestDTO.reimbursement_type_id.GetValueOrDefault());

            if (reimbursementType.Type == "Food-Water")
            {
                if (reimburseRequestDTO.invoice_amount < 1000 || reimburseRequestDTO.invoice_amount > 1500)
                    throw new ArgumentException("Invoice amount must be between 1000 and 1500 for Food and Water");
            }
            else if (reimbursementType.Type == "Laundry")
            {
                if (reimburseRequestDTO.invoice_amount < 250 || reimburseRequestDTO.invoice_amount > 500)
                    throw new ArgumentException("Invoice amount must be between 250 and 500 for Laundry");
            }
            else if (reimbursementType.Type == "LocalTravel")
            {
                if (reimburseRequestDTO.invoice_amount > 1000)
                    throw new ArgumentException("Local travel amount must not exceed 1000 per day");
            }

            // ====================================================
            // STEP 5 - Handle File
            // Generate unique name, build path, save file to disk
            // ====================================================

            // Generate unique file name to avoid overwriting existing files
            Random random = new Random();
            int randomNumber = random.Next(100000, 999999);
            string originalFileName = Path.GetFileNameWithoutExtension(reimburseRequestDTO.document.FileName);
            string fileName = $"{randomNumber}_{reimburseRequestDTO.travel_request_id}_{originalFileName}.pdf";

            // Build folder path → wwwroot/uploads/reimbursements/
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "reimbursementDocs").Replace("\\", "/");

            // Create folder if it does not exist
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Combine folder path and file name to get full save path
            string fullFilePath = Path.Combine(folderPath, fileName).Replace("\\", "/"); ;

            // Convert IFormFile to bytes and save physically to disk
            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await reimburseRequestDTO.document.CopyToAsync(stream);

            }

            // Request DTO to Entity
            var reimburseEntity = _mapper.Map<ReimbursementRequest>(reimburseRequestDTO);

            reimburseEntity.DocumentUrl = fullFilePath;
            reimburseEntity.RequestDate = DateOnly.FromDateTime(DateTime.Now);
            reimburseEntity.Status = "New";
            
            var saveReimburseEntity = await _reimbursementRepo.AddReimbursement(reimburseEntity);

            // Entity to DTO to show response

            var reimburseResponseDTO = _mapper.Map<ReimbursementResponseDTO>(saveReimburseEntity);
            return reimburseResponseDTO;
        }

        public async Task<List<ReimbursementResponseDTO>> GetAllReimbursementRequest(int trid)
        {
            var result = await _reimbursementRepo.GetAllReimbursementRequest(trid);
            return _mapper.Map<List<ReimbursementResponseDTO>>(result);
        }

        public async Task<ReimbursementResponseDTO> GetReimbursementDetails(int reimbursementid)
        {
            var reimburseDetails = await _reimbursementRepo.GetReimbursementDetails(reimbursementid);
            return _mapper.Map<ReimbursementResponseDTO>(reimburseDetails);

        }

        public async Task<ReimbursementResponseDTO> ProcessReimbursemnet(int reimbursementid, ReimbursementProcessRequestDTO reimburseProcessDTO)
        {
            var result = await _reimbursementRepo.GetReimbursementDetails(reimbursementid);
            
            if (!result.Status.Equals("New", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only Reimbursement requests with status 'New' can be updated.");

            var validStatuses = new[] { "Approved", "Rejected" };

            if (!validStatuses.Contains(reimburseProcessDTO.status, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Status must be either Approved or Rejected.");

            if(reimburseProcessDTO.status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                result.Status = "Approved";
                result.Remarks = null;
                result.RequestProcessedByEmployeeId = reimburseProcessDTO.request_processed_by_employee_id;
                result.RequestProcessedOn = DateOnly.FromDateTime(DateTime.Now);
            }
            else if(reimburseProcessDTO.status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(reimburseProcessDTO.remarks))
                    throw new ArgumentException("Remarks are mandatory when rejecteting a reimbursement");

                result.Status = "Rejected";
                result.Remarks = reimburseProcessDTO.remarks;
                result.RequestProcessedByEmployeeId = reimburseProcessDTO.request_processed_by_employee_id;
                result.RequestProcessedOn = DateOnly.FromDateTime(DateTime.Now);
            }

            var updatedResult = await _reimbursementRepo.ProcessReimbursemnet(result);

            return _mapper.Map<ReimbursementResponseDTO>(updatedResult);
        }
    }
}
