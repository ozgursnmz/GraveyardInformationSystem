namespace Graveyard.API.Models;

// Soft-delete (arsivleme) alanlari. Ana tablolar kalici silinmez; IsArchived=1 isaretlenir.
// EF Core bu ozellikleri ayni adli kolonlara convention ile otomatik esler.

public partial class Person
{
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
}

public partial class GravePlot
{
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
}

public partial class GraveOwner
{
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
}

public partial class DeceasedPerson
{
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
}
