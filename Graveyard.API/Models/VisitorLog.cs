using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class VisitorLog
{
    public string VisitId { get; set; } = null!;

    public string? VisitorName { get; set; }

    public DateOnly? VisitDate { get; set; }

    public TimeOnly? ArrivalTime { get; set; }

    public TimeOnly? DepartureTime { get; set; }

    public string? Purpose { get; set; }

    public string? PlotNumber { get; set; }

    public virtual GravePlot? PlotNumberNavigation { get; set; }
}
