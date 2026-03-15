namespace ETD_Portal.HR_Management.DTOs.RequestDTO
{
    public class GradeHistoryRequestDTO
    {
        public int Id { get; set; }

        public DateOnly? AssignedOn { get; set; }

        public int? EmployeeId { get; set; }

        public int? GradeId { get; set; }
    }
}
