using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.HR_Management.DTOs.RequestDTO
{
    public class GradeUpdateRequestDTO
    {
        public string? first_name { get; set; }

        public string? last_name { get; set; }
        public string? phone_number { get; set; }

        public string? email_address { get; set; }

        public string? role { get; set; }

        public int? current_grade_id { get; set; }
    }
}
