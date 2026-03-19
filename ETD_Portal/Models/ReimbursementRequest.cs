using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("ReimbursementRequest")]
public partial class ReimbursementRequest
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("travel_request_id")]
    public int? TravelRequestId { get; set; }

    [Column("request_raised_by_employee_id")]
    public int? RequestRaisedByEmployeeId { get; set; }

    [Column("request_date")]
    public DateOnly RequestDate { get; set; }

    [Column("reimbursement_type_id")]
    public int? ReimbursementTypeId { get; set; }

    [Column("invoice_no")]
    [StringLength(20)]
    [Unicode(false)]
    public string? InvoiceNo { get; set; }

    [Column("invoice_date")]
    public DateOnly? InvoiceDate { get; set; }

    [Column("invoice_amount")]
    public int? InvoiceAmount { get; set; }

    [Column("document_url")]
    [StringLength(100)]
    [Unicode(false)]
    public string? DocumentUrl { get; set; }

    [Column("request_processed_on")]
    public DateOnly? RequestProcessedOn { get; set; }

    [Column("request_processed_by_employee_id")]
    public int? RequestProcessedByEmployeeId { get; set; }

    [Column("status")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column("remarks")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Remarks { get; set; }

    [ForeignKey("ReimbursementTypeId")]
    [InverseProperty("ReimbursementRequests")]
    public virtual ReimbursementType? ReimbursementType { get; set; }

    [ForeignKey("RequestProcessedByEmployeeId")]
    [InverseProperty("ReimbursementRequestRequestProcessedByEmployees")]
    public virtual User? RequestProcessedByEmployee { get; set; }

    [ForeignKey("RequestRaisedByEmployeeId")]
    [InverseProperty("ReimbursementRequestRequestRaisedByEmployees")]
    public virtual User? RequestRaisedByEmployee { get; set; }

    [ForeignKey("TravelRequestId")]
    [InverseProperty("ReimbursementRequests")]
    public virtual TravelRequest? TravelRequest { get; set; }
}
