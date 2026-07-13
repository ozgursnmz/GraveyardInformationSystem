using System;

namespace Graveyard.API.Models;

public partial class AuditLog
{
    public int AuditId { get; set; }

    public string? Username { get; set; }

    public string? Action { get; set; }

    public string? Entity { get; set; }

    public string? EntityKey { get; set; }

    public DateTime Timestamp { get; set; }
}
