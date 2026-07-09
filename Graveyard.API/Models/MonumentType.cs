using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class MonumentType
{
    public string MonumentCode { get; set; } = null!;

    public string? Material { get; set; }

    public string? Style { get; set; }

    public double? MaxHeight { get; set; }

    public double? BaseWidth { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<GravePlot> GravePlots { get; set; } = new List<GravePlot>();
}
