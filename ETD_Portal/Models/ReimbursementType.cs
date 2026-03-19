using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("ReimbursementType")]
public partial class ReimbursementType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("type")]
    [StringLength(25)]
    [Unicode(false)]
    public string? Type { get; set; }

    [InverseProperty("ReimbursementType")]
    public virtual ICollection<ReimbursementRequest> ReimbursementRequests { get; set; } = new List<ReimbursementRequest>();
}
