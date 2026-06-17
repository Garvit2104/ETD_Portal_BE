using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.DTOs.ResquestDTO
{
    public class ReimbursementProcessRequestDTO
    {

        public int? request_processed_by_employee_id { get; set; }

        public string? status { get; set; }

        public string? remarks { get; set; }
    }
}
