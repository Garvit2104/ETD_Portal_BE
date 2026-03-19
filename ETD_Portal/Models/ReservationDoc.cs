using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("ReservationDoc")]
public partial class ReservationDoc
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("reservation_id")]
    public int? ReservationId { get; set; }

    [Column("document_url")]
    [StringLength(100)]
    [Unicode(false)]
    public string? DocumentUrl { get; set; }

    [ForeignKey("ReservationId")]
    [InverseProperty("ReservationDocs")]
    public virtual Reservation? Reservation { get; set; }
}
