using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonumentTypesController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public MonumentTypesController(GraveyardDbContext context) => _context = context;

    // GET: api/MonumentTypes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MonumentType>>> GetAll()
        => await _context.MonumentTypes.ToListAsync();

    // GET: api/MonumentTypes/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<MonumentType>> GetById(string id)
    {
        var item = await _context.MonumentTypes.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/MonumentTypes
    [HttpPost]
    public async Task<ActionResult<MonumentType>> Create(MonumentType item)
    {
        _context.MonumentTypes.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.MonumentCode }, item);
    }

    // PUT: api/MonumentTypes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, MonumentType item)
    {
        if (id != item.MonumentCode) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.MonumentTypes.AnyAsync(e => e.MonumentCode == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/MonumentTypes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.MonumentTypes.FindAsync(id);
        if (item == null) return NotFound();
        _context.MonumentTypes.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
