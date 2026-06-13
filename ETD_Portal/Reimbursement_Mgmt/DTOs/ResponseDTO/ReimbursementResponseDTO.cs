using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.DTOs.ResponseDTO
{
    public class ReimbursementResponseDTO
    {
        public int? id { get; set; }

        public int? travel_request_id { get; set; }

        public int? request_raised_by_employee_id { get; set; }

        public DateOnly? request_date { get; set; }

        public int? reimbursement_type_id { get; set; }

        public string? reimbursement_type_name { get; set; }  // from navigation property

        public string? invoice_no { get; set; }

        public DateOnly? invoice_date { get; set; }

        public int? invoice_amount { get; set; }


        public string? status { get; set; }

        public string? remarks { get; set; }

    }
}
