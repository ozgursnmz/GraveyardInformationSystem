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

    // GET: api/Stats  -> dashboard kart ozetleri (herkese acik)
    [HttpGet]
    public async Task<ActionResult<StatsDto>> Get()
    {
        var total = await _context.GravePlots.CountAsync();
        var occupied = await _context.GravePlots.CountAsync(p => p.Status == "Occupied");
        var available = await _context.GravePlots.CountAsync(p => p.Status == "Available");
        var reserved = await _context.GravePlots.CountAsync(p => p.Status == "Reserved");
        var deceased = await _context.DeceasedPeople.CountAsync();
        var revenue = await _context.Payments.SumAsync(p => (double?)p.Amount) ?? 0;
        var rate = total > 0 ? Math.Round((double)occupied / total * 100, 1) : 0;

        return new StatsDto(total, occupied, available, reserved, rate, deceased, revenue);
    }
}
