using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class Payment
{
    public string ReceiptNo { get; set; } = null!;

    public double? Amount { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Currency { get; set; }

    public string? BillingAddress { get; set; }

    public string? OwnerSsn { get; set; }

    public virtual GraveOwner? OwnerSsnNavigation { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
