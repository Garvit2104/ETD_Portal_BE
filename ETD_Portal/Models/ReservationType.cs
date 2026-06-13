using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETD_Portal.Models;

public partial class ReservationType
{
    [Column("type_id")]
    public int TypeId { get; set; }

    [Column("type_name")]
    public string? TypeName { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
