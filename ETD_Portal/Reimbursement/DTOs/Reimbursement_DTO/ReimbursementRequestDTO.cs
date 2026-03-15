using System.ComponentModel.DataAnnotations;

namespace Reimbursement__Managment.DTOs.Reimbursement
{
    public class ReimbursementRequestDTO
    {
        public int travel_request_id { get; set; }

        public int request_raised_by_employee_id { get; set; }

        public int reimbursement_type_id { get; set; }

        [StringLength(30)]
        public string invoice_no { get; set; }

        public DateOnly invoice_date { get; set; }

        public int invoice_amount { get; set; }

        public IFormFile document { get; set; }
        public string document_url { get; internal set; }
    }
}
