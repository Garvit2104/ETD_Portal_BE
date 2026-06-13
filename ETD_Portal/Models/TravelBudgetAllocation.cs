using System;
using System.Collections.Generic;

namespace ETD_Portal.Models;

public partial class TravelBudgetAllocation
{
    public int Id { get; set; }

    public int? TravelRequestId { get; set; }

    public int? ApprovedBudget { get; set; }

    public string? ApprovedModeOfTravel { get; set; }

    public string? ApprovedHotelStarRating { get; set; }

    public virtual TravelRequest? TravelRequest { get; set; }
}
