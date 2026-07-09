using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class DeceasedPerson
{
    public string Ssn { get; set; } = null!;

    public DateOnly? DateOfDeath { get; set; }

    public string? CauseOfDeath { get; set; }

    public string? Religion { get; set; }

    public string? VeteranStatus { get; set; }

    public string? FuneralPreferences { get; set; }

    public string? PlotNumber { get; set; }

    public string? PermitNumber { get; set; }

    public virtual ICollection<FuneralService> FuneralServices { get; set; } = new List<FuneralService>();

    public virtual BurialPermit? PermitNumberNavigation { get; set; }

    public virtual GravePlot? PlotNumberNavigation { get; set; }

    public virtual Person SsnNavigation { get; set; } = null!;
}
