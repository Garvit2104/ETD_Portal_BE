namespace ETD_Portal.HR_Management.DTOs.RequestDTO
{
    public class UserRequestDTO
    {
        public string? first_name { get; set; }

        public string? last_name { get; set; }
        public string? phone_number { get; set; }

        public string? email_address { get; set; }

        public string? role { get; set; }

        public int? current_grade_id { get; set; }

        public string? password_hash { get; set; } = null;

    }
}
