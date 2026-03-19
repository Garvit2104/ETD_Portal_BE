using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("Grade")]
public partial class Grade
{
    [Key]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Name { get; set; }

    [InverseProperty("Grade")]
    public virtual ICollection<GradeHistory> GradeHistories { get; set; } = new List<GradeHistory>();

    [InverseProperty("CurrentGrade")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
