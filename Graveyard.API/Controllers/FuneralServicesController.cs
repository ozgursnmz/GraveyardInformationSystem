using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuneralServicesController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public FuneralServicesController(GraveyardDbContext context) => _context = context;

    // GET: api/FuneralServices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FuneralService>>> GetAll()
        => await _context.FuneralServices.ToListAsync();

    // GET: api/FuneralServices/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<FuneralService>> GetById(string id)
    {
        var item = await _context.FuneralServices.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/FuneralServices
    [HttpPost]
    public async Task<ActionResult<FuneralService>> Create(FuneralService item)
    {
        _context.FuneralServices.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.ServiceId }, item);
    }

    // PUT: api/FuneralServices/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, FuneralService item)
    {
        if (id != item.ServiceId) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.FuneralServices.AnyAsync(e => e.ServiceId == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/FuneralServices/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.FuneralServices.FindAsync(id);
        if (item == null) return NotFound();
        _context.FuneralServices.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
