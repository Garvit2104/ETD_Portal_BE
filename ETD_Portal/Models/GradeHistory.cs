using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("GradeHistory")]
public partial class GradeHistory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("assigned_on")]
    public DateOnly? AssignedOn { get; set; }

    [Column("employee_id")]
    public int? EmployeeId { get; set; }

    [Column("grade_id")]
    public int? GradeId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("GradeHistories")]
    public virtual User? Employee { get; set; }

    [ForeignKey("GradeId")]
    [InverseProperty("GradeHistories")]
    public virtual Grade? Grade { get; set; }
}
