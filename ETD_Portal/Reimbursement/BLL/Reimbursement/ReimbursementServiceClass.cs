using Microsoft.AspNetCore.Mvc;
using Reimbursement__Managment.DTOs.Reimbursement;
using Reimbursement__Managment.DAL.Reimbursements;
using Reimbursement__Managment.DTOs.Reimbursement_DTO;
using Reimbursement__Managment.DAL;
using Reimbursement__Managment.BLL.HRClientSerives;
using Reimbursement__Managment.BLL.ClientServices;
using Reimbursement__Managment.DTOs.TPClinet_DTO;
using Reimbursement__Managment.Models;
namespace Reimbursement__Managment.BLL.Reimbursement
{
    public class ReimbursementServiceClass : IReimbursementService
    {
        private readonly IReimbursementRepo _reimburseRepo;
        private readonly IReimbursementTypeRepo _reimburseTypeRepo;
        private readonly HRClientServiceClass _hrClient;
        private readonly TPClientServiceClass _tpClient;
        public ReimbursementServiceClass(IReimbursementRepo _reimburseRepo, IReimbursementTypeRepo _reimburseTypeRepo, HRClientServiceClass _hrClient, TPClientServiceClass _tpClient)
        {
            this._reimburseRepo = _reimburseRepo;
            this._reimburseTypeRepo = _reimburseTypeRepo;
            this._hrClient = _hrClient;
            this._tpClient = _tpClient;
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

            var employee = await _hrClient.ValidateGetEmployeeId(reimburseRequestDTO.request_raised_by_employee_id);
            if (employee == null)
            {
                throw new Exception("Invalid employee id");
            }

            // step 3 -> Validate Invoice Details
            var travelDetails = await _tpClient.GetTravellingDates(reimburseRequestDTO.travel_request_id);
            if (travelDetails == null)
            {
                throw new Exception("Invalid travel request id");
            }
            DateTime fromDate = (DateTime)travelDetails.from_date;
            DateTime toDate = (DateTime)travelDetails.to_date;

            var invoiceDateTime = reimburseRequestDTO.invoice_date.ToDateTime(TimeOnly.MinValue);
            var validInvoiceDate = fromDate < invoiceDateTime && invoiceDateTime < toDate;

            if (!validInvoiceDate)
            {
                throw new Exception("Invoice date must be between from date and to date of travel request");
            }

            //  Per Day Expense Validation for Local Travel
            var reimbursementType = await _reimburseTypeRepo.GetTypeById(reimburseRequestDTO.reimbursement_type_id);

            if (reimbursementType.Type == "Food" || reimbursementType.Type == "Water")
            {
                if (reimburseRequestDTO.invoice_amount < 1000 || reimburseRequestDTO.invoice_amount > 1500)
                    throw new Exception("Invoice amount must be between 1000 and 1500 for Food and Water");
            }
            else if (reimbursementType.Type == "Laundry")
            {
                // BUG 3 FIX - Was "Laundary" (typo), corrected to "Laundry"
                // Also amount was 200 minimum, document says 250 minimum
                if (reimburseRequestDTO.invoice_amount < 250 || reimburseRequestDTO.invoice_amount > 500)
                    throw new Exception("Invoice amount must be between 250 and 500 for Laundry");
            }
            else if (reimbursementType.Type == "LocalTravel")
            {
                // BUG 4 FIX - Was just throwing exception without checking amount
                // Should validate amount does not exceed 1000
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
            reimburseEntity.InvoiceNo = reimburseRequestDTO.invoice_no;
            reimburseEntity.InvoiceDate = reimburseRequestDTO.invoice_date;
            reimburseEntity.InvoiceAmount = reimburseRequestDTO.invoice_amount;
            reimburseEntity.DocumentUrl = fullFilePath;
            reimburseEntity.RequestDate = DateOnly.FromDateTime(DateTime.Now);
            reimburseEntity.Status = "New";
            reimburseEntity.RequestProcessedOn = null;
            reimburseEntity.RequestProcessedByEmployeeId = null;
            reimburseEntity.Remarks = null;

            var saveReimburseEntity = await _reimburseRepo.AddReimbursement(reimburseEntity);

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


        public async Task<IEnumerable<ReimbursementRequest>> GetAllReimbursementRequest(int travelrequestid)
        {
            var tRequest = _tpClient.GetTravelRequestById(travelrequestid);
            if(tRequest == null)
            {
                throw new Exception("Travel Request with this id is exisit");
            }

            var reimburseRequest = await _reimburseRepo.GetAllReimbursementRequest(travelrequestid);

            List<ReimbursementResponseDTO> requestList = new List<ReimbursementResponseDTO>();

               foreach(var item in reimburseRequest)
            {
                ReimbursementResponseDTO reimburseResponseDTO = new ReimbursementResponseDTO
                {
                    id = item.Id,
                    request_raised_by_employee_id = item.RequestRaisedByEmployeeId,
                    travel_request_id = item.TravelRequestId,
                    reimbursement_type_id = item.ReimbursementTypeId,
                    invoice_no = item.InvoiceNo,
                    invoice_date = item.InvoiceDate,
                    invoice_amount = item.InvoiceAmount,
                    document_url = item.DocumentUrl,
                    request_date = item.RequestDate,
                    status = item.Status,
                    remarks = item.Remarks
                };
                requestList.Add(reimburseResponseDTO);
            }
            
        }
    }
}

