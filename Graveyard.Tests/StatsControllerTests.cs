using Graveyard.API.Controllers;
using Graveyard.API.Dtos;
using Graveyard.API.Models;
using Xunit;

namespace Graveyard.Tests;

public class StatsControllerTests
{
    // Parsel durumlarina gore sayimlarin ve doluluk oraninin dogru hesaplandigini dogrular.
    [Fact]
    public async Task Get_ComputesPlotCountsAndOccupancyRate()
    {
        using var db = TestDb.Create();
        db.GravePlots.AddRange(
            new GravePlot { PlotNumber = "P1", Status = "Occupied" },
            new GravePlot { PlotNumber = "P2", Status = "Occupied" },
            new GravePlot { PlotNumber = "P3", Status = "Available" },
            new GravePlot { PlotNumber = "P4", Status = "Reserved" });
        await db.SaveChangesAsync();

        var result = await new StatsController(db).Get(0);
        var stats = Assert.IsType<StatsDto>(result.Value);

        Assert.Equal(4, stats.TotalPlots);
        Assert.Equal(2, stats.OccupiedPlots);
        Assert.Equal(1, stats.AvailablePlots);
        Assert.Equal(1, stats.ReservedPlots);
        Assert.Equal(50.0, stats.OccupancyRate); // 2/4 = %50
    }

    // Gelir - gider = net bakiye mantigini dogrular.
    [Fact]
    public async Task Get_ComputesNetAsRevenueMinusExpense()
    {
        using var db = TestDb.Create();
        db.Payments.Add(new Payment { ReceiptNo = "R1", Amount = 1000, PaymentDate = new DateOnly(2024, 5, 1), PaymentMethod = "Cash" });
        db.MaintenanceLogs.Add(new MaintenanceLog { PlotNumber = "P1", LogNo = "L1", Cost = 300, LogDate = new DateOnly(2024, 5, 2) });
        await db.SaveChangesAsync();

        var stats = (await new StatsController(db).Get(0)).Value!;

        Assert.Equal(1000, stats.TotalRevenue);
        Assert.Equal(300, stats.TotalExpense);
        Assert.Equal(700, stats.NetProfit);
    }

    // Bolge doluluk yuzdesinin mevcut/kapasite orani olarak dondugunu dogrular.
    [Fact]
    public async Task Charts_ReturnsZoneOccupancyPercent()
    {
        using var db = TestDb.Create();
        db.CemeteryZones.Add(new CemeteryZone { ZoneId = "A", Name = "A Bölgesi", TotalCapacity = 200, CurrentOccupancy = 50 });
        await db.SaveChangesAsync();

        var charts = (await new StatsController(db).Charts(0)).Value!;

        var zone = Assert.Single(charts.ZoneOccupancyPct);
        Assert.Equal("A Bölgesi", zone.Label);
        Assert.Equal(25.0, zone.Value); // 50/200 = %25
    }

    // Ziyaretlerin haftanin gunlerine gore gruplandigini dogrular (0=Pazar).
    [Fact]
    public async Task Charts_GroupsVisitsByWeekday()
    {
        using var db = TestDb.Create();
        // 2024-07-08 Pazartesi (DayOfWeek=1), 2024-07-15 de Pazartesi -> ayni gune 2 ziyaret
        db.VisitorLogs.AddRange(
            new VisitorLog { VisitId = "V1", VisitDate = new DateOnly(2024, 7, 8) },
            new VisitorLog { VisitId = "V2", VisitDate = new DateOnly(2024, 7, 15) },
            new VisitorLog { VisitId = "V3", VisitDate = new DateOnly(2024, 7, 14) }); // Pazar (0)
        await db.SaveChangesAsync();

        var charts = (await new StatsController(db).Charts(0)).Value!;

        var monday = charts.VisitsByWeekday.Single(x => x.Label == "1");
        var sunday = charts.VisitsByWeekday.Single(x => x.Label == "0");
        Assert.Equal(2, monday.Value);
        Assert.Equal(1, sunday.Value);
    }
}
