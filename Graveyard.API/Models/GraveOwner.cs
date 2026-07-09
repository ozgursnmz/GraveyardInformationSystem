using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class GraveOwner
{
    public string Ssn { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? OwnerType { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    public string? Relationship { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual Person? SsnNavigation { get; set; }
}
