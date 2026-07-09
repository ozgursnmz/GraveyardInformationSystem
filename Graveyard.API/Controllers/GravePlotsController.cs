using Microsoft.AspNetCore.Authorization;
using Graveyard.API.Data;
using Graveyard.API.Dtos;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GravePlotsController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public GravePlotsController(GraveyardDbContext context) => _context = context;

    // GET: api/GravePlots  -> tum mezar yerleri
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GravePlot>>> GetAll()
        => await _context.GravePlots.ToListAsync();

    // GET: api/GravePlots/PLT001  -> tek mezar yeri
    [HttpGet("{id}")]
    public async Task<ActionResult<GravePlot>> GetById(string id)
    {
        var plot = await _context.GravePlots.FindAsync(id);
        if (plot == null) return NotFound();
        return plot;
    }

    // GET: api/GravePlots/search?status=Available&zoneID=Z001&plotNumber=PLT0  -> filtreli arama
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<GravePlot>>> Search(
        string? status, string? zoneID, string? plotNumber)
    {
        var query = _context.GravePlots.AsQueryable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status == status);
        if (!string.IsNullOrEmpty(zoneID))
            query = query.Where(p => p.ZoneId == zoneID);
        if (!string.IsNullOrWhiteSpace(plotNumber))
            query = query.Where(p => p.PlotNumber.Contains(plotNumber));
        return await query.ToListAsync();
    }

    // GET: api/GravePlots/map  -> harita/mezar sorgulama icin (halka acik, sadece ad+konum)
    [AllowAnonymous]
    [HttpGet("map")]
    public async Task<ActionResult<IEnumerable<MapPlotDto>>> Map()
    {
        return await _context.GravePlots
            .Where(p => p.Latitude != null && p.Longitude != null)
            .Select(p => new MapPlotDto(
                p.PlotNumber, p.Latitude, p.Longitude, p.Status,
                p.Zone!.Name,
                p.DeceasedPerson != null
                    ? p.DeceasedPerson.SsnNavigation.FirstName + " " + p.DeceasedPerson.SsnNavigation.LastName
                    : null,
                (p.DeceasedPerson != null && p.DeceasedPerson.DateOfDeath != null)
                    ? p.DeceasedPerson.DateOfDeath.Value.Year
                    : (int?)null))
            .ToListAsync();
    }

    // POST: api/GravePlots  -> yeni mezar yeri ekle
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<GravePlot>> Create(GravePlot plot)
    {
        _context.GravePlots.Add(plot);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = plot.PlotNumber }, plot);
    }

    // PUT: api/GravePlots/PLT001  -> guncelle
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, GravePlot plot)
    {
        if (id != plot.PlotNumber)
            return BadRequest("URL'deki id ile govdedeki PlotNumber uyusmuyor.");

        _context.Entry(plot).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.GravePlots.AnyAsync(p => p.PlotNumber == id))
                return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/GravePlots/PLT001  -> sil
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var plot = await _context.GravePlots.FindAsync(id);
        if (plot == null) return NotFound();
        _context.GravePlots.Remove(plot);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
