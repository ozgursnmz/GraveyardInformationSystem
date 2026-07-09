using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GraveOwnersController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public GraveOwnersController(GraveyardDbContext context) => _context = context;

    // GET: api/GraveOwners
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GraveOwner>>> GetAll()
        => await _context.GraveOwners.ToListAsync();

    // GET: api/GraveOwners/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<GraveOwner>> GetById(string id)
    {
        var item = await _context.GraveOwners.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/GraveOwners
    [HttpPost]
    public async Task<ActionResult<GraveOwner>> Create(GraveOwner item)
    {
        _context.GraveOwners.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Ssn }, item);
    }

    // PUT: api/GraveOwners/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, GraveOwner item)
    {
        if (id != item.Ssn) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.GraveOwners.AnyAsync(e => e.Ssn == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/GraveOwners/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.GraveOwners.FindAsync(id);
        if (item == null) return NotFound();
        _context.GraveOwners.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
