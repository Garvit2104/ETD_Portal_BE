namespace ETD_Portal.HR_Management.DTOs.ResponseDTO
{
    public class UserResponseDTO
    {
        public int? employee_id { get; set; }

        public string? first_name { get; set; }

        public string? last_name { get; set; }
        public string? phone_number { get; set; }

        public string? email_address { get; set; }

        public string? role { get; set; }

        public int? current_grade_id { get; set; }

        public string current_grade_name { get; set; }
    }
}
