using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class MaintenanceLog
{
    public string PlotNumber { get; set; } = null!;

    public string LogNo { get; set; } = null!;

    public DateOnly? LogDate { get; set; }

    public string? TaskDescription { get; set; }

    public double? HoursSpent { get; set; }

    public double? Cost { get; set; }

    public string? EmployeeId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual GravePlot? PlotNumberNavigation { get; set; }
}
