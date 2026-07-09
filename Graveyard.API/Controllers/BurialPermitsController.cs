using Microsoft.AspNetCore.Authorization;
using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BurialPermitsController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public BurialPermitsController(GraveyardDbContext context) => _context = context;

    // GET: api/BurialPermits
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BurialPermit>>> GetAll()
        => await _context.BurialPermits.ToListAsync();

    // GET: api/BurialPermits/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<BurialPermit>> GetById(string id)
    {
        var item = await _context.BurialPermits.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/BurialPermits
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<BurialPermit>> Create(BurialPermit item)
    {
        _context.BurialPermits.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.PermitNumber }, item);
    }

    // PUT: api/BurialPermits/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, BurialPermit item)
    {
        if (id != item.PermitNumber) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.BurialPermits.AnyAsync(e => e.PermitNumber == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/BurialPermits/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.BurialPermits.FindAsync(id);
        if (item == null) return NotFound();
        _context.BurialPermits.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
