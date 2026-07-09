using System;
using System.Collections.Generic;

namespace Graveyard.API.Models;

public partial class Employee
{
    public string EmployeeId { get; set; } = null!;

    public string? Ssn { get; set; }

    public string? JobTitle { get; set; }

    public DateOnly? HireDate { get; set; }

    public double? Salary { get; set; }

    public string? Shift { get; set; }

    public string? SupervisorEmployeeId { get; set; }

    public virtual ICollection<Employee> InverseSupervisorEmployee { get; set; } = new List<Employee>();

    public virtual ICollection<MaintenanceLog> MaintenanceLogs { get; set; } = new List<MaintenanceLog>();

    public virtual Person? SsnNavigation { get; set; }

    public virtual Employee? SupervisorEmployee { get; set; }
}
