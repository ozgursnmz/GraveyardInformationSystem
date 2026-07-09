using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class FuneralService
{
    public string ServiceId { get; set; } = null!;

    public DateOnly? ServiceDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? ServiceType { get; set; }

    public int? ExpectedAttendees { get; set; }

    public string? DeceasedSsn { get; set; }

    public virtual DeceasedPerson? DeceasedSsnNavigation { get; set; }
}
