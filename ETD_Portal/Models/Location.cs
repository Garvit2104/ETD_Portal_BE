using System;
using System.Collections.Generic;

namespace ETD_Portal.Models;

public partial class Location
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<TravelRequest> TravelRequests { get; set; } = new List<TravelRequest>();
}
