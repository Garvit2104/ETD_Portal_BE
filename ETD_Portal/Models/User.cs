using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("User")]
[Index("EmailAddress", Name = "UQ_users_email", IsUnique = true)]
[Index("PhoneNumber", Name = "UQ_users_phone", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Column("first_name")]
    [StringLength(15)]
    [Unicode(false)]
    public string? FirstName { get; set; }

    [Column("last_name")]
    [StringLength(10)]
    [Unicode(false)]
    public string? LastName { get; set; }

    [Column("phone_number")]
    [StringLength(10)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [Column("email_address")]
    [StringLength(50)]
    [Unicode(false)]
    public string? EmailAddress { get; set; }

    [Column("role")]
    [StringLength(15)]
    [Unicode(false)]
    public string? Role { get; set; }

    [Column("current_grade_id")]
    public int? CurrentGradeId { get; set; }

    [ForeignKey("CurrentGradeId")]
    [InverseProperty("Users")]
    public virtual Grade? CurrentGrade { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<GradeHistory> GradeHistories { get; set; } = new List<GradeHistory>();

    [InverseProperty("RequestProcessedByEmployee")]
    public virtual ICollection<ReimbursementRequest> ReimbursementRequestRequestProcessedByEmployees { get; set; } = new List<ReimbursementRequest>();

    [InverseProperty("RequestRaisedByEmployee")]
    public virtual ICollection<ReimbursementRequest> ReimbursementRequestRequestRaisedByEmployees { get; set; } = new List<ReimbursementRequest>();

    [InverseProperty("ReservationDoneByEmployee")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [InverseProperty("RaisedByEmployee")]
    public virtual ICollection<TravelRequest> TravelRequestRaisedByEmployees { get; set; } = new List<TravelRequest>();

    [InverseProperty("ToBeApprovedByHr")]
    public virtual ICollection<TravelRequest> TravelRequestToBeApprovedByHrs { get; set; } = new List<TravelRequest>();
}
