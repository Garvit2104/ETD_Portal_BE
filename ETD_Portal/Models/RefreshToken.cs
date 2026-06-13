using System;
using System.Collections.Generic;

namespace ETD_Portal.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Employee { get; set; } = null!;
}
