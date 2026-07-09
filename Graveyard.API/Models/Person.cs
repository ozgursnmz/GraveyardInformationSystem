using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class Person
{
    public string Ssn { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? MotherName { get; set; }

    public string? FatherName { get; set; }

    public virtual DeceasedPerson? DeceasedPerson { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual GraveOwner? GraveOwner { get; set; }
}
