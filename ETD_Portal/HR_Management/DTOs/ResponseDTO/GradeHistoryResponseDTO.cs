namespace ETD_Portal.HR_Management.DTOs.ResponseDTO
{
    public class GradeHistoryResponseDTO
    {
        public int Id { get; set; }

        public DateOnly? AssignedOn { get; set; }

        public int? EmployeeId { get; set; }

        public int? GradeId { get; set; }
    }
}
