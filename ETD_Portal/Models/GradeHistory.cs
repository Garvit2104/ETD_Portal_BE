using System;
using System.Collections.Generic;

namespace ETD_Portal.Models;

public partial class GradeHistory
{
    public int Id { get; set; }

    public DateOnly? AssignedOn { get; set; }

    public int? EmployeeId { get; set; }

    public int? GradeId { get; set; }

    public virtual User? Employee { get; set; }

    public virtual Grade? Grade { get; set; }
}
