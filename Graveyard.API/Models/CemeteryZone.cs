using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class CemeteryZone
{
    public string ZoneId { get; set; } = null!;

    public string? Name { get; set; }

    public string? ReligionType { get; set; }

    public int? TotalCapacity { get; set; }

    public int? CurrentOccupancy { get; set; }

    public string? GroundType { get; set; }

    public virtual ICollection<GravePlot> GravePlots { get; set; } = new List<GravePlot>();
}
