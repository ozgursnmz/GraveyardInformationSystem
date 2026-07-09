using Microsoft.AspNetCore.Authorization;
using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitorLogsController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public VisitorLogsController(GraveyardDbContext context) => _context = context;

    // GET: api/VisitorLogs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VisitorLog>>> GetAll()
        => await _context.VisitorLogs.ToListAsync();

    // GET: api/VisitorLogs/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<VisitorLog>> GetById(string id)
    {
        var item = await _context.VisitorLogs.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/VisitorLogs
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<VisitorLog>> Create(VisitorLog item)
    {
        _context.VisitorLogs.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.VisitId }, item);
    }

    // PUT: api/VisitorLogs/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, VisitorLog item)
    {
        if (id != item.VisitId) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.VisitorLogs.AnyAsync(e => e.VisitId == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/VisitorLogs/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.VisitorLogs.FindAsync(id);
        if (item == null) return NotFound();
        _context.VisitorLogs.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
