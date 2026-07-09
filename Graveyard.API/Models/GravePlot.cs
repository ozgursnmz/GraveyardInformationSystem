using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class GravePlot
{
    public string PlotNumber { get; set; } = null!;

    public double? Length { get; set; }

    public double? Width { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Status { get; set; }

    public string? ZoneId { get; set; }

    public string? MonumentCode { get; set; }

    public virtual DeceasedPerson? DeceasedPerson { get; set; }

    public virtual ICollection<MaintenanceLog> MaintenanceLogs { get; set; } = new List<MaintenanceLog>();

    public virtual MonumentType? MonumentCodeNavigation { get; set; }

    public virtual Reservation? Reservation { get; set; }

    public virtual ICollection<VisitorLog> VisitorLogs { get; set; } = new List<VisitorLog>();

    public virtual CemeteryZone? Zone { get; set; }
}
