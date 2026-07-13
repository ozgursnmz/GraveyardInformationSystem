using System;
using System.Collections.Generic;
using Graveyard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Data;

public partial class GraveyardDbContext : DbContext
{
    public GraveyardDbContext(DbContextOptions<GraveyardDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BurialPermit> BurialPermits { get; set; }

    public virtual DbSet<CemeteryZone> CemeteryZones { get; set; }

    public virtual DbSet<DeceasedPerson> DeceasedPeople { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<FuneralService> FuneralServices { get; set; }

    public virtual DbSet<GraveOwner> GraveOwners { get; set; }

    public virtual DbSet<GravePlot> GravePlots { get; set; }

    public virtual DbSet<MaintenanceLog> MaintenanceLogs { get; set; }

    public virtual DbSet<MonumentType> MonumentTypes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<VisitorLog> VisitorLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__APP_USER__1788CCAC9A7B3DA9");

            entity.ToTable("APP_USER");

            entity.HasIndex(e => e.Username, "UQ__APP_USER__536C85E43DF4B3B2").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Admin");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId);
            entity.ToTable("AUDIT_LOG");
            entity.Property(e => e.AuditId).HasColumnName("AuditID");
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Action).HasMaxLength(20);
            entity.Property(e => e.Entity).HasMaxLength(50);
            entity.Property(e => e.EntityKey).HasMaxLength(100);
            entity.Property(e => e.Timestamp).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<BurialPermit>(entity =>
        {
            entity.HasKey(e => e.PermitNumber).HasName("PK__BURIAL_P__DA3C94EF82E3AFEA");

            entity.ToTable("BURIAL_PERMIT");

            entity.Property(e => e.PermitNumber).HasMaxLength(15);
            entity.Property(e => e.AuthorizedSignature).HasMaxLength(50);
            entity.Property(e => e.IssuingAuthority).HasMaxLength(50);
        });

        modelBuilder.Entity<CemeteryZone>(entity =>
        {
            entity.HasKey(e => e.ZoneId).HasName("PK__CEMETERY__601667955AC00AD7");

            entity.ToTable("CEMETERY_ZONE");

            entity.Property(e => e.ZoneId)
                .HasMaxLength(10)
                .HasColumnName("ZoneID");
            entity.Property(e => e.CurrentOccupancy).HasDefaultValue(0);
            entity.Property(e => e.GroundType).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ReligionType).HasMaxLength(30);
        });

        modelBuilder.Entity<DeceasedPerson>(entity =>
        {
            entity.HasKey(e => e.Ssn).HasName("PK__DECEASED__CA1E8E3D13D47867");

            entity.ToTable("DECEASED_PERSON");

            entity.HasIndex(e => e.PlotNumber, "UQ__DECEASED__0DFF8347EDEC6AAD").IsUnique();

            entity.HasIndex(e => e.PermitNumber, "UQ__DECEASED__DA3C94EE1975C3A7").IsUnique();

            entity.Property(e => e.Ssn)
                .HasMaxLength(11)
                .HasColumnName("SSN");
            entity.Property(e => e.CauseOfDeath).HasMaxLength(100);
            entity.Property(e => e.FuneralPreferences).HasMaxLength(200);
            entity.Property(e => e.PermitNumber).HasMaxLength(15);
            entity.Property(e => e.PlotNumber).HasMaxLength(15);
            entity.Property(e => e.Religion).HasMaxLength(30);
            entity.Property(e => e.VeteranStatus)
                .HasMaxLength(10)
                .HasDefaultValue("No");

            entity.HasOne(d => d.PermitNumberNavigation).WithOne(p => p.DeceasedPerson)
                .HasForeignKey<DeceasedPerson>(d => d.PermitNumber)
                .HasConstraintName("FK__DECEASED___Permi__5629CD9C");

            entity.HasOne(d => d.PlotNumberNavigation).WithOne(p => p.DeceasedPerson)
                .HasForeignKey<DeceasedPerson>(d => d.PlotNumber)
                .HasConstraintName("FK__DECEASED___PlotN__5535A963");

            entity.HasOne(d => d.SsnNavigation).WithOne(p => p.DeceasedPerson)
                .HasForeignKey<DeceasedPerson>(d => d.Ssn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DECEASED_PE__SSN__5441852A");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__EMPLOYEE__7AD04FF132363871");

            entity.ToTable("EMPLOYEE");

            entity.HasIndex(e => e.Ssn, "UQ__EMPLOYEE__CA1E8E3C9D6562E0").IsUnique();

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .HasColumnName("EmployeeID");
            entity.Property(e => e.JobTitle).HasMaxLength(50);
            entity.Property(e => e.Shift).HasMaxLength(20);
            entity.Property(e => e.Ssn)
                .HasMaxLength(11)
                .HasColumnName("SSN");
            entity.Property(e => e.SupervisorEmployeeId)
                .HasMaxLength(10)
                .HasColumnName("SupervisorEmployeeID");

            entity.HasOne(d => d.SsnNavigation).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.Ssn)
                .HasConstraintName("FK__EMPLOYEE__SSN__403A8C7D");

            entity.HasOne(d => d.SupervisorEmployee).WithMany(p => p.InverseSupervisorEmployee)
                .HasForeignKey(d => d.SupervisorEmployeeId)
                .HasConstraintName("FK__EMPLOYEE__Superv__412EB0B6");
        });

        modelBuilder.Entity<FuneralService>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__FUNERAL___C51BB0EA58E47E8D");

            entity.ToTable("FUNERAL_SERVICE");

            entity.Property(e => e.ServiceId)
                .HasMaxLength(15)
                .HasColumnName("ServiceID");
            entity.Property(e => e.DeceasedSsn)
                .HasMaxLength(11)
                .HasColumnName("DeceasedSSN");
            entity.Property(e => e.ExpectedAttendees).HasDefaultValue(0);
            entity.Property(e => e.ServiceType).HasMaxLength(30);

            entity.HasOne(d => d.DeceasedSsnNavigation).WithMany(p => p.FuneralServices)
                .HasForeignKey(d => d.DeceasedSsn)
                .HasConstraintName("FK__FUNERAL_S__Decea__59FA5E80");
        });

        modelBuilder.Entity<GraveOwner>(entity =>
        {
            entity.HasKey(e => e.Ssn).HasName("PK__GRAVE_OW__CA1E8E3DE8E63F80");

            entity.ToTable("GRAVE_OWNER");

            entity.Property(e => e.Ssn)
                .HasMaxLength(11)
                .HasColumnName("SSN");
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.OwnerType)
                .HasMaxLength(20)
                .HasDefaultValue("Individual");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.SsnNavigation).WithOne(p => p.GraveOwner)
                .HasForeignKey<GraveOwner>(d => d.Ssn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GRAVE_OWNER__SSN__3B75D760");
        });

        modelBuilder.Entity<GravePlot>(entity =>
        {
            entity.HasKey(e => e.PlotNumber).HasName("PK__GRAVE_PL__0DFF83465C26BE54");

            entity.ToTable("GRAVE_PLOT");

            entity.Property(e => e.PlotNumber).HasMaxLength(15);
            entity.Property(e => e.MonumentCode).HasMaxLength(10);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Available");
            entity.Property(e => e.ZoneId)
                .HasMaxLength(10)
                .HasColumnName("ZoneID");

            entity.HasOne(d => d.MonumentCodeNavigation).WithMany(p => p.GravePlots)
                .HasForeignKey(d => d.MonumentCode)
                .HasConstraintName("FK__GRAVE_PLO__Monum__4BAC3F29");

            entity.HasOne(d => d.Zone).WithMany(p => p.GravePlots)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__GRAVE_PLO__ZoneI__4AB81AF0");
        });

        modelBuilder.Entity<MaintenanceLog>(entity =>
        {
            entity.HasKey(e => new { e.PlotNumber, e.LogNo }).HasName("PK__MAINTENA__381ADF944BF061AF");

            entity.ToTable("MAINTENANCE_LOG");

            entity.Property(e => e.PlotNumber).HasMaxLength(15);
            entity.Property(e => e.LogNo).HasMaxLength(15);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .HasColumnName("EmployeeID");
            entity.Property(e => e.TaskDescription).HasMaxLength(200);

            entity.HasOne(d => d.Employee).WithMany(p => p.MaintenanceLogs)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__MAINTENAN__Emplo__6D0D32F4");

            entity.HasOne(d => d.PlotNumberNavigation).WithMany(p => p.MaintenanceLogs)
                .HasForeignKey(d => d.PlotNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MAINTENAN__PlotN__6C190EBB");
        });

        modelBuilder.Entity<MonumentType>(entity =>
        {
            entity.HasKey(e => e.MonumentCode).HasName("PK__MONUMENT__0A8B09A11CA5504A");

            entity.ToTable("MONUMENT_TYPE");

            entity.Property(e => e.MonumentCode).HasMaxLength(10);
            entity.Property(e => e.Color).HasMaxLength(20);
            entity.Property(e => e.Material).HasMaxLength(30);
            entity.Property(e => e.Style).HasMaxLength(30);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.ReceiptNo).HasName("PK__PAYMENT__CC0B72A792582D5B");

            entity.ToTable("PAYMENT");

            entity.Property(e => e.ReceiptNo).HasMaxLength(15);
            entity.Property(e => e.BillingAddress).HasMaxLength(100);
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("TRY");
            entity.Property(e => e.OwnerSsn)
                .HasMaxLength(11)
                .HasColumnName("OwnerSSN");
            entity.Property(e => e.PaymentMethod).HasMaxLength(20);

            entity.HasOne(d => d.OwnerSsnNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OwnerSsn)
                .HasConstraintName("FK__PAYMENT__OwnerSS__628FA481");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Ssn).HasName("PK__PERSON__CA1E8E3DB68D61EE");

            entity.ToTable("PERSON");

            entity.Property(e => e.Ssn)
                .HasMaxLength(11)
                .HasColumnName("SSN");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasDefaultValue("Unknown");
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__RESERVAT__B7EE5F04E3A4DF1D");

            entity.ToTable("RESERVATION");

            entity.HasIndex(e => e.PlotNumber, "UQ__RESERVAT__0DFF8347A8CF7868").IsUnique();

            entity.Property(e => e.ReservationId)
                .HasMaxLength(15)
                .HasColumnName("ReservationID");
            entity.Property(e => e.Notes).HasMaxLength(200);
            entity.Property(e => e.OwnerSsn)
                .HasMaxLength(11)
                .HasColumnName("OwnerSSN");
            entity.Property(e => e.PlotNumber).HasMaxLength(15);
            entity.Property(e => e.ReceiptNo).HasMaxLength(15);
            entity.Property(e => e.ReservationType).HasMaxLength(20);

            entity.HasOne(d => d.OwnerSsnNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.OwnerSsn)
                .HasConstraintName("FK__RESERVATI__Owner__66603565");

            entity.HasOne(d => d.PlotNumberNavigation).WithOne(p => p.Reservation)
                .HasForeignKey<Reservation>(d => d.PlotNumber)
                .HasConstraintName("FK__RESERVATI__PlotN__68487DD7");

            entity.HasOne(d => d.ReceiptNoNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ReceiptNo)
                .HasConstraintName("FK__RESERVATI__Recei__6754599E");
        });

        modelBuilder.Entity<VisitorLog>(entity =>
        {
            entity.HasKey(e => e.VisitId).HasName("PK__VISITOR___4D3AA1BE7DE9843C");

            entity.ToTable("VISITOR_LOG");

            entity.Property(e => e.VisitId)
                .HasMaxLength(15)
                .HasColumnName("VisitID");
            entity.Property(e => e.PlotNumber).HasMaxLength(15);
            entity.Property(e => e.Purpose).HasMaxLength(100);
            entity.Property(e => e.VisitorName).HasMaxLength(100);

            entity.HasOne(d => d.PlotNumberNavigation).WithMany(p => p.VisitorLogs)
                .HasForeignKey(d => d.PlotNumber)
                .HasConstraintName("FK__VISITOR_L__PlotN__5DCAEF64");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
