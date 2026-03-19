using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("Location")]
public partial class Location
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Name { get; set; }

    [InverseProperty("Location")]
    public virtual ICollection<TravelRequest> TravelRequests { get; set; } = new List<TravelRequest>();
}
