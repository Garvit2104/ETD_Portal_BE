using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("Reservation")]
public partial class Reservation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("reservation_done_by_employee_id")]
    public int? ReservationDoneByEmployeeId { get; set; }

    [Column("travel_request_id")]
    public int? TravelRequestId { get; set; }

    [Column("reservation_type_id")]
    public int? ReservationTypeId { get; set; }

    [Column("created_on")]
    public DateOnly CreatedOn { get; set; }

    [Column("reservation_done_with_entity")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ReservationDoneWithEntity { get; set; }

    [Column("reservation_date")]
    public DateOnly? ReservationDate { get; set; }

    [Column("amount")]
    public int? Amount { get; set; }

    [Column("confirmation_id")]
    [StringLength(10)]
    [Unicode(false)]
    public string? ConfirmationId { get; set; }

    [Column("remarks")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Remarks { get; set; }

    [InverseProperty("Reservation")]
    public virtual ICollection<ReservationDoc> ReservationDocs { get; set; } = new List<ReservationDoc>();

    [ForeignKey("ReservationDoneByEmployeeId")]
    [InverseProperty("Reservations")]
    public virtual User? ReservationDoneByEmployee { get; set; }

    [ForeignKey("ReservationTypeId")]
    [InverseProperty("Reservations")]
    public virtual ReservationType? ReservationType { get; set; }

    [ForeignKey("TravelRequestId")]
    [InverseProperty("Reservations")]
    public virtual TravelRequest? TravelRequest { get; set; }
}
