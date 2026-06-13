using System;
using System.Collections.Generic;

namespace ETD_Portal.Models;

public partial class ReimbursementType
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<ReimbursementRequest> ReimbursementRequests { get; set; } = new List<ReimbursementRequest>();
}
