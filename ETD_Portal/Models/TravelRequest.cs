using System;
using System.Collections.Generic;

namespace ETD_Portal.Models;

public partial class TravelRequest
{
    public int? RequestId { get; set; }

    public int? RaisedByEmployeeId { get; set; }

    public int? ToBeApprovedByHrId { get; set; }

    public DateOnly RequestRaisedOn { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    public string? PurposeOfTravel { get; set; }

    public int? LocationId { get; set; }

    public string? RequestStatus { get; set; }

    public DateOnly? RequestApprovedOn { get; set; }

    public string? Priority { get; set; }

    public virtual Location? Location { get; set; }

    public virtual User? RaisedByEmployee { get; set; }

    public virtual ICollection<ReimbursementRequest> ReimbursementRequests { get; set; } = new List<ReimbursementRequest>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual User? ToBeApprovedByHr { get; set; }

    public virtual ICollection<TravelBudgetAllocation> TravelBudgetAllocations { get; set; } = new List<TravelBudgetAllocation>();
}
