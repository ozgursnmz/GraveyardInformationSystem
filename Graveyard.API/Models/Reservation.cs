using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class Reservation
{
    public string ReservationId { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? ReservationType { get; set; }

    public string? Notes { get; set; }

    public string? OwnerSsn { get; set; }

    public string? ReceiptNo { get; set; }

    public string? PlotNumber { get; set; }

    public virtual GraveOwner? OwnerSsnNavigation { get; set; }

    public virtual GravePlot? PlotNumberNavigation { get; set; }

    public virtual Payment? ReceiptNoNavigation { get; set; }
}
