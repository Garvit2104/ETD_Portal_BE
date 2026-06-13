using System;
using System.Collections.Generic;

namespace ETD_Portal.Models;

public partial class ReservationDoc
{
    public int Id { get; set; }

    public int? ReservationId { get; set; }

    public string? DocumentUrl { get; set; }

    public virtual Reservation? Reservation { get; set; }
}
