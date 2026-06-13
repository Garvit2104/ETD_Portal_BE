using System;
using System.Collections.Generic;

namespace ETD_Portal.Models;

public partial class User
{
    public int EmployeeId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? EmailAddress { get; set; }

    public string? Role { get; set; }

    public int? CurrentGradeId { get; set; }

    public string? PasswordHash { get; set; }

    public virtual Grade? CurrentGrade { get; set; }

    public virtual ICollection<GradeHistory> GradeHistories { get; set; } = new List<GradeHistory>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<ReimbursementRequest> ReimbursementRequestRequestProcessedByEmployees { get; set; } = new List<ReimbursementRequest>();

    public virtual ICollection<ReimbursementRequest> ReimbursementRequestRequestRaisedByEmployees { get; set; } = new List<ReimbursementRequest>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<TravelRequest> TravelRequestRaisedByEmployees { get; set; } = new List<TravelRequest>();

    public virtual ICollection<TravelRequest> TravelRequestToBeApprovedByHrs { get; set; } = new List<TravelRequest>();
}
