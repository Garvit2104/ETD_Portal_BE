using System;
using System.Collections.Generic;

namespace ETD_Portal.Models;

public partial class ReimbursementRequest
{
    public int Id { get; set; }

    public int? TravelRequestId { get; set; }

    public int? RequestRaisedByEmployeeId { get; set; }

    public DateOnly RequestDate { get; set; }

    public int? ReimbursementTypeId { get; set; }

    public string? InvoiceNo { get; set; }

    public DateOnly? InvoiceDate { get; set; }

    public int? InvoiceAmount { get; set; }

    public string? DocumentUrl { get; set; }

    public DateOnly? RequestProcessedOn { get; set; }

    public int? RequestProcessedByEmployeeId { get; set; }

    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public virtual ReimbursementType? ReimbursementType { get; set; }

    public virtual User? RequestProcessedByEmployee { get; set; }

    public virtual User? RequestRaisedByEmployee { get; set; }

    public virtual TravelRequest? TravelRequest { get; set; }
}
