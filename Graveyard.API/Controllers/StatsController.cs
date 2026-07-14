using Graveyard.API.Data;
using Graveyard.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public StatsController(GraveyardDbContext context) => _context = context;

    // Donem baslangicini hesapla. months=0 -> tum zamanlar.
    // Referans nokta: sistemdeki en son kayit tarihi (ornek veri 2024 oldugu icin).
    private async Task<DateOnly> PeriodStartAsync(int months)
    {
        if (months <= 0) return DateOnly.MinValue;

        var maxPay = await _context.Payments.MaxAsync(p => (DateOnly?)p.PaymentDate);
        var maxDeath = await _context.DeceasedPeople.MaxAsync(d => (DateOnly?)d.DateOfDeath);
        var maxLog = await _context.MaintenanceLogs.MaxAsync(m => (DateOnly?)m.LogDate);

        var anchors = new[] { maxPay, maxDeath, maxLog }
            .Where(x => x.HasValue).Select(x => x!.Value).ToList();
        var anchor = anchors.Any() ? anchors.Max() : DateOnly.FromDateTime(DateTime.UtcNow);
        return anchor.AddMonths(-months);
    }

    // GET: api/Stats?months=3  -> dashboard kart ozetleri
    [HttpGet]
    public async Task<ActionResult<StatsDto>> Get(int months = 0)
    {
        var start = await PeriodStartAsync(months);

        // Durum bazli (zamansiz) - her zaman toplam
        var total = await _context.GravePlots.CountAsync();
        var occupied = await _context.GravePlots.CountAsync(p => p.Status == "Occupied");
        var available = await _context.GravePlots.CountAsync(p => p.Status == "Available");
        var reserved = await _context.GravePlots.CountAsync(p => p.Status == "Reserved");
        var rate = total > 0 ? Math.Round((double)occupied / total * 100, 1) : 0;

        // Zaman bazli - donem filtreli
        var deceased = await _context.DeceasedPeople
            .CountAsync(d => d.DateOfDeath != null && d.DateOfDeath >= start);
        var revenue = await _context.Payments
            .Where(p => p.PaymentDate != null && p.PaymentDate >= start)
            .SumAsync(p => (double?)p.Amount) ?? 0;
        var expense = await _context.MaintenanceLogs
            .Where(m => m.LogDate != null && m.LogDate >= start)
            .SumAsync(m => (double?)m.Cost) ?? 0;

        return new StatsDto(total, occupied, available, reserved, rate, deceased,
                            revenue, expense, revenue - expense);
    }

    // GET: api/Stats/charts?months=3  -> dashboard grafikleri
    [HttpGet("charts")]
    public async Task<ActionResult<ChartsDto>> Charts(int months = 0)
    {
        var start = await PeriodStartAsync(months);

        // Bolgelere gore doluluk (zamansiz)
        var zones = await _context.CemeteryZones
            .Select(z => new LabelValue(z.Name ?? z.ZoneId, z.CurrentOccupancy ?? 0))
            .ToListAsync();

        // Aylara gore vefat (donem filtreli)
        var deathsRaw = await _context.DeceasedPeople
            .Where(d => d.DateOfDeath != null && d.DateOfDeath >= start)
            .GroupBy(d => d.DateOfDeath!.Value.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .OrderBy(x => x.Month)
            .ToListAsync();
        var deaths = deathsRaw.Select(x => new LabelValue(x.Month.ToString(), x.Count)).ToList();

        // Odeme yontemi dagilimi (donem filtreli)
        var methods = await _context.Payments
            .Where(p => p.PaymentDate != null && p.PaymentDate >= start)
            .GroupBy(p => p.PaymentMethod)
            .Select(g => new LabelValue(g.Key ?? "—", g.Count()))
            .ToListAsync();

        // Finansal ozet (donem filtreli)
        var income = await _context.Payments
            .Where(p => p.PaymentDate != null && p.PaymentDate >= start)
            .SumAsync(p => (double?)p.Amount) ?? 0;
        var expense = await _context.MaintenanceLogs
            .Where(m => m.LogDate != null && m.LogDate >= start)
            .SumAsync(m => (double?)m.Cost) ?? 0;

        // Aylara gore bakim maliyeti (donem filtreli)
        var maintRaw = await _context.MaintenanceLogs
            .Where(m => m.LogDate != null && m.LogDate >= start)
            .GroupBy(m => m.LogDate!.Value.Month)
            .Select(g => new { Month = g.Key, Cost = g.Sum(x => x.Cost) })
            .OrderBy(x => x.Month)
            .ToListAsync();
        var maint = maintRaw.Select(x => new LabelValue(x.Month.ToString(), x.Cost ?? 0)).ToList();

        // Bolge doluluk yuzdesi (zamansiz) = mevcut doluluk / toplam kapasite
        var zoneCap = await _context.CemeteryZones
            .Select(z => new { Name = z.Name ?? z.ZoneId, z.TotalCapacity, z.CurrentOccupancy })
            .ToListAsync();
        var zonePct = zoneCap.Select(z => new LabelValue(
            z.Name,
            z.TotalCapacity is > 0
                ? Math.Round((double)(z.CurrentOccupancy ?? 0) / z.TotalCapacity!.Value * 100, 0)
                : 0)).ToList();

        // Gunlere gore ziyaret sayisi (donem filtreli) - 0=Pazar..6=Cumartesi
        var visitDates = await _context.VisitorLogs
            .Where(v => v.VisitDate != null && v.VisitDate >= start)
            .Select(v => v.VisitDate!.Value)
            .ToListAsync();
        var visits = visitDates
            .GroupBy(d => (int)d.DayOfWeek)
            .Select(g => new LabelValue(g.Key.ToString(), g.Count()))
            .ToList();

        return new ChartsDto(zones, deaths, methods, income, expense, maint, zonePct, visits);
    }
}
