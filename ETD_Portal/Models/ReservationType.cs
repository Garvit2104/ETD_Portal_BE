using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("ReservationType")]
public partial class ReservationType
{
    [Key]
    [Column("type_id")]
    public int TypeId { get; set; }

    [Column("type_name")]
    [StringLength(25)]
    [Unicode(false)]
    public string? TypeName { get; set; }

    [InverseProperty("ReservationType")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
