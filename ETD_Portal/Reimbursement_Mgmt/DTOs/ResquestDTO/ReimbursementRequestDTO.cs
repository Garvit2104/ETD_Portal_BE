using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.DTOs.ResquestDTO
{
    public class ReimbursementRequestDTO
    {
        public int? travel_request_id { get; set; }

        public int? request_raised_by_employee_id { get; set; }

        public int? reimbursement_type_id { get; set; }

        public DateOnly? invoice_date { get; set; }

        public int? invoice_amount { get; set; }

     
        public IFormFile? document { get; set; }




        

    }
}
