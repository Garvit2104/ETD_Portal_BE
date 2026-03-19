using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Models;

[Table("TravelBudgetAllocation")]
public partial class TravelBudgetAllocation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("travel_request_id")]
    public int? TravelRequestId { get; set; }

    [Column("approved_budget")]
    public int? ApprovedBudget { get; set; }

    [Column("approved_mode_of_travel")]
    [StringLength(10)]
    [Unicode(false)]
    public string? ApprovedModeOfTravel { get; set; }

    [Column("approved_hotel_star_rating")]
    [StringLength(6)]
    [Unicode(false)]
    public string? ApprovedHotelStarRating { get; set; }

    [ForeignKey("TravelRequestId")]
    [InverseProperty("TravelBudgetAllocations")]
    public virtual TravelRequest? TravelRequest { get; set; }
}
