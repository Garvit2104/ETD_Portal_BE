using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("TravelRequest")]
public partial class TravelRequest
{
    [Key]
    [Column("request_id")]
    public int RequestId { get; set; }

    [Column("raised_by_employee_id")]
    public int? RaisedByEmployeeId { get; set; }

    [Column("to_be_approved_by_hr_id")]
    public int? ToBeApprovedByHrId { get; set; }

    [Column("request_raised_on")]
    public DateOnly RequestRaisedOn { get; set; }

    [Column("from_date")]
    public DateOnly? FromDate { get; set; }

    [Column("to_date")]
    public DateOnly? ToDate { get; set; }

    [Column("purpose_of_travel")]
    [StringLength(100)]
    [Unicode(false)]
    public string? PurposeOfTravel { get; set; }

    [Column("location_id")]
    public int? LocationId { get; set; }

    [Column("request_status")]
    [StringLength(15)]
    [Unicode(false)]
    public string? RequestStatus { get; set; }

    [Column("request_approved_on")]
    public DateOnly? RequestApprovedOn { get; set; }

    [Column("priority")]
    [StringLength(6)]
    [Unicode(false)]
    public string? Priority { get; set; }

    [ForeignKey("LocationId")]
    [InverseProperty("TravelRequests")]
    public virtual Location? Location { get; set; }

    [ForeignKey("RaisedByEmployeeId")]
    [InverseProperty("TravelRequestRaisedByEmployees")]
    public virtual User? RaisedByEmployee { get; set; }

    [InverseProperty("TravelRequest")]
    public virtual ICollection<ReimbursementRequest> ReimbursementRequests { get; set; } = new List<ReimbursementRequest>();

    [InverseProperty("TravelRequest")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("ToBeApprovedByHrId")]
    [InverseProperty("TravelRequestToBeApprovedByHrs")]
    public virtual User? ToBeApprovedByHr { get; set; }

    [InverseProperty("TravelRequest")]
    public virtual ICollection<TravelBudgetAllocation> TravelBudgetAllocations { get; set; } = new List<TravelBudgetAllocation>();
}
