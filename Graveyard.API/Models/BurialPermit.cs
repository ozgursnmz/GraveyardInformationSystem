using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class BurialPermit
{
    public string PermitNumber { get; set; } = null!;

    public string? IssuingAuthority { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public string? AuthorizedSignature { get; set; }

    public virtual DeceasedPerson? DeceasedPerson { get; set; }
}
