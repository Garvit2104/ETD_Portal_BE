using System;
using System.Collections.Generic;

namespace ETD_Portal.Models;

public partial class Grade
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<GradeHistory> GradeHistories { get; set; } = new List<GradeHistory>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
