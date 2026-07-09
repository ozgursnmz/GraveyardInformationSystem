using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CemeteryZonesController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public CemeteryZonesController(GraveyardDbContext context) => _context = context;

    // GET: api/CemeteryZones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CemeteryZone>>> GetAll()
        => await _context.CemeteryZones.ToListAsync();

    // GET: api/CemeteryZones/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CemeteryZone>> GetById(string id)
    {
        var item = await _context.CemeteryZones.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/CemeteryZones
    [HttpPost]
    public async Task<ActionResult<CemeteryZone>> Create(CemeteryZone item)
    {
        _context.CemeteryZones.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.ZoneId }, item);
    }

    // PUT: api/CemeteryZones/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, CemeteryZone item)
    {
        if (id != item.ZoneId) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.CemeteryZones.AnyAsync(e => e.ZoneId == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/CemeteryZones/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.CemeteryZones.FindAsync(id);
        if (item == null) return NotFound();
        _context.CemeteryZones.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
