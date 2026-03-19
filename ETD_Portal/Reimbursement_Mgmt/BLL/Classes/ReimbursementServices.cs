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

namespace ETD_Portal.Reimbursement_Mgmt.BLL.Classes
{
    public class ReimbursementServices : IReimbursementServices
    {
        private readonly IReimbursementRepo _reimbursementRepo;
        private readonly IUserServices _userServices;
        private readonly IUserRepo _userRepo;
        private readonly ITravelRequestRepo _trRepo;
        private readonly ITravelRequestServices _travelRequestServices;
        private readonly IReimbursementTypeRepo _reimbursementTypeRepo;
        public ReimbursementServices(IReimbursementRepo _reimbursementRepo, 
            IUserServices _userServices, ITravelRequestServices _travelRequestServices, 
            IReimbursementTypeRepo _reimbursementTypeRepo, IUserRepo _userRepo, ITravelRequestRepo _trRepo)
        {
            this._reimbursementRepo = _reimbursementRepo;
            this._userServices = _userServices;
            this._travelRequestServices = _travelRequestServices;   
            this._reimbursementTypeRepo = _reimbursementTypeRepo;
            this._userRepo = _userRepo;
            this._trRepo = _trRepo;
        }

        public async Task<ReimbursementResponseDTO> AddReimbursement(ReimbursementRequestDTO reimburseRequestDTO)
        {
            // step 1 -> Validate the file
            if (reimburseRequestDTO.document == null || reimburseRequestDTO.document.Length == 0)
            {
                throw new Exception("File is required");
            }
            if (!reimburseRequestDTO.document.ContentType.Equals("application/pdf"))
            {
                throw new Exception("Only pdf documents are allowed");
            }
            if (reimburseRequestDTO.document.Length > 256 * 1024)
            {
                throw new Exception("Document Size must not exceed 256 kb");
            }
            // step 2 -> Valide employee_id

            User employee = await _userRepo.GetEmployeeById(reimburseRequestDTO.request_raised_by_employee_id.GetValueOrDefault());
            if (employee == null)
            {
                throw new Exception("Invalid employee id");
            }

            // step 3 -> Validate Invoice Details
            TravelRequest travelDetails = await _trRepo.getTravelRequestById(reimburseRequestDTO.travel_request_id.GetValueOrDefault());
            if (travelDetails == null)
            {
                throw new Exception("Invalid travel request id");
            }
            DateTime fromDate = travelDetails.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            DateTime toDate = travelDetails.ToDate.Value.ToDateTime(TimeOnly.MinValue);

            var invoiceDateTime = reimburseRequestDTO.invoice_date.Value.ToDateTime(TimeOnly.MinValue);
            var validInvoiceDate = fromDate < invoiceDateTime && invoiceDateTime < toDate;

            if (!validInvoiceDate)
            {
                throw new Exception("Invoice date must be between from date and to date of travel request");
            }

            //  Per Day Expense Validation for Local Travel
            var reimbursementType = await _reimbursementTypeRepo.GetTypeById(reimburseRequestDTO.reimbursement_type_id.GetValueOrDefault());

            if (reimbursementType.Type == "Food-Water")
            {
                if (reimburseRequestDTO.invoice_amount < 1000 || reimburseRequestDTO.invoice_amount > 1500)
                    throw new Exception("Invoice amount must be between 1000 and 1500 for Food and Water");
            }
            else if (reimbursementType.Type == "Laundry")
            {
                if (reimburseRequestDTO.invoice_amount < 250 || reimburseRequestDTO.invoice_amount > 500)
                    throw new Exception("Invoice amount must be between 250 and 500 for Laundry");
            }
            else if (reimbursementType.Type == "LocalTravel")
            {
                if (reimburseRequestDTO.invoice_amount > 1000)
                    throw new Exception("Local travel amount must not exceed 1000 per day");
            }

            // ====================================================
            // STEP 5 - Handle File
            // Generate unique name, build path, save file to disk
            // ====================================================

            // Generate unique file name to avoid overwriting existing files
            string originalFileName = Path.GetFileNameWithoutExtension(reimburseRequestDTO.document.FileName);
            string fileName = $"{Guid.NewGuid()}_{reimburseRequestDTO.travel_request_id}_{originalFileName}.pdf";

            // Build folder path → wwwroot/uploads/reimbursements/
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "reimbursementDocs");

            // Create folder if it does not exist
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Combine folder path and file name to get full save path
            string fullFilePath = Path.Combine(folderPath, fileName);

            // Convert IFormFile to bytes and save physically to disk
            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await reimburseRequestDTO.document.CopyToAsync(stream);

            }

            // Request DTO to Entity
            ReimbursementRequest reimburseEntity = new ReimbursementRequest();
            reimburseEntity.RequestRaisedByEmployeeId = reimburseRequestDTO.request_raised_by_employee_id;
            reimburseEntity.TravelRequestId = reimburseRequestDTO.travel_request_id;
            reimburseEntity.ReimbursementTypeId = reimburseRequestDTO.reimbursement_type_id;
            //reimburseEntity.InvoiceNo = reimburseRequestDTO.invoice_no;
            reimburseEntity.InvoiceDate = reimburseRequestDTO.invoice_date;
            reimburseEntity.InvoiceAmount = reimburseRequestDTO.invoice_amount;
            //reimburseEntity.DocumentUrl = fullFilePath;
            reimburseEntity.RequestDate = DateOnly.FromDateTime(DateTime.Now);
            reimburseEntity.Status = "New";
            reimburseEntity.RequestProcessedOn = null;
            reimburseEntity.RequestProcessedByEmployeeId = null;
            reimburseEntity.Remarks = null;

            var saveReimburseEntity = await _reimbursementRepo.AddReimbursement(reimburseEntity);

            // Entity to DTO to show response

            ReimbursementResponseDTO reimburseResponseDTO = new ReimbursementResponseDTO
            {
                id = saveReimburseEntity.Id,
                request_raised_by_employee_id = saveReimburseEntity.RequestRaisedByEmployeeId,
                travel_request_id = saveReimburseEntity.TravelRequestId,
                reimbursement_type_id = saveReimburseEntity.ReimbursementTypeId,
                invoice_no = saveReimburseEntity.InvoiceNo,
                invoice_date = saveReimburseEntity.InvoiceDate,
                invoice_amount = saveReimburseEntity.InvoiceAmount,
                document_url = saveReimburseEntity.DocumentUrl,
                request_date = saveReimburseEntity.RequestDate,
                status = saveReimburseEntity.Status,
                remarks = saveReimburseEntity.Remarks
            };
            return reimburseResponseDTO;
        }

        public async Task<List<ReimbursementResponseDTO>> GetAllReimbursementRequest(int trid)
        {
            var result = await _reimbursementRepo.GetAllReimbursementRequest(trid);
            List<ReimbursementResponseDTO> allReimbursementList = new List<ReimbursementResponseDTO>();
            foreach(var item in result)
            {
                ReimbursementResponseDTO reimbursementResponse = new ReimbursementResponseDTO
                {
                    id = item.Id,
                    request_raised_by_employee_id = item.RequestRaisedByEmployeeId,
                    travel_request_id = item.TravelRequestId,
                    reimbursement_type_id = item.ReimbursementTypeId,
                    invoice_date = item.InvoiceDate,
                    invoice_amount = item.InvoiceAmount,
                    request_date = item.RequestDate,
                    status = item.Status,
                    remarks = item.Remarks
                };
                allReimbursementList.Add(reimbursementResponse);
            }
            return allReimbursementList;
        }

        public async Task<ReimbursementResponseDTO> GetReimbursementDetails(int reimbursementid)
        {
            var reimburseDetails = await _reimbursementRepo.GetReimbursementDetails(reimbursementid);

            ReimbursementResponseDTO reimburseDetailResponseDTO = new ReimbursementResponseDTO
            {
                id = reimburseDetails.Id,
                request_raised_by_employee_id = reimburseDetails.RequestRaisedByEmployeeId,
                travel_request_id = reimburseDetails.TravelRequestId,
                reimbursement_type_id = reimburseDetails.ReimbursementTypeId,
                invoice_date = reimburseDetails.InvoiceDate,
                invoice_amount = reimburseDetails.InvoiceAmount,
                request_date = reimburseDetails.RequestDate,
                status = reimburseDetails.Status,
                remarks = reimburseDetails.Remarks
            };

            return reimburseDetailResponseDTO;

        }

        public async Task<ReimbursementResponseDTO> ProcessReimbursemnet(int reimbursementid, ReimbursementRequestDTO reimburseDTO)
        {
            var result = await _reimbursementRepo.GetReimbursementDetails(reimbursementid);
            if(result == null)
                throw new KeyNotFoundException($"Reimbursement request with ID {reimbursementid} not found.");

            if (!result.Status.Equals("New", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only Reimbursement requests with status 'New' can be updated.");

            var validStatuses = new[] { "Approved", "Rejected" };

            if (!validStatuses.Contains(reimburseDTO.status, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Status must be either Approved or Rejected.");

            result.Status = reimburseDTO.status;

            if (result.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                result.RequestProcessedOn = DateOnly.FromDateTime(DateTime.Now);

            if (result.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                result.Remarks = reimburseDTO.remarks;
                result.RequestProcessedOn = DateOnly.FromDateTime(DateTime.Now);

            

            var updatedResult = await _reimbursementRepo.ProcessReimbursemnet(result);

            ReimbursementResponseDTO updatedReimburseDetailResponseDTO = new ReimbursementResponseDTO
            {
                id = result.Id,
                request_raised_by_employee_id = result.RequestRaisedByEmployeeId,
                travel_request_id = result.TravelRequestId,
                reimbursement_type_id = result.ReimbursementTypeId,
                invoice_date = result.InvoiceDate,
                invoice_amount = result.InvoiceAmount,
                request_date = result.RequestDate,
                status = result.Status,
                remarks = result.Remarks
            };

            return updatedReimburseDetailResponseDTO;
        }


    }
}
