namespace Reimbursement__Managment.DTOs.Reimbursement_DTO
{
    public class ReimbursementResponseDTO
    {
        public int id { get; set; }

        public int travel_request_id { get; set; }

        public int request_raised_by_employee_id { get; set; }

        public DateOnly request_date { get; set; }

        public int reimbursement_type_id { get; set; }

        public string reimbursement_type_name { get; set; }  // from navigation property

        public string invoice_no { get; set; }

        public DateOnly invoice_date { get; set; }

        public int invoice_amount { get; set; }

        public string document_url { get; set; }

        public DateOnly? request_processed_on { get; set; }

        public int? request_processed_by_employee_id { get; set; }

        public string status { get; set; }

        public string? remarks { get; set; }
    }
}
