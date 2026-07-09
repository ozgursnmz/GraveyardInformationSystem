using Microsoft.AspNetCore.Authorization;
using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeceasedPeopleController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public DeceasedPeopleController(GraveyardDbContext context) => _context = context;

    // GET: api/DeceasedPeople
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeceasedPerson>>> GetAll()
        => await _context.DeceasedPeople.ToListAsync();

    // GET: api/DeceasedPeople/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<DeceasedPerson>> GetById(string id)
    {
        var item = await _context.DeceasedPeople.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // GET: api/DeceasedPeople/search?fromDate=2024-01-01&toDate=2024-06-30&religion=Islam&plotNumber=PLT001
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<DeceasedPerson>>> Search(
        DateOnly? fromDate, DateOnly? toDate, string? religion, string? plotNumber)
    {
        var query = _context.DeceasedPeople.AsQueryable();
        if (fromDate.HasValue)
            query = query.Where(d => d.DateOfDeath >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(d => d.DateOfDeath <= toDate.Value);
        if (!string.IsNullOrWhiteSpace(religion))
            query = query.Where(d => d.Religion == religion);
        if (!string.IsNullOrWhiteSpace(plotNumber))
            query = query.Where(d => d.PlotNumber!.Contains(plotNumber));
        return await query.ToListAsync();
    }

    // POST: api/DeceasedPeople
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<DeceasedPerson>> Create(DeceasedPerson item)
    {
        _context.DeceasedPeople.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Ssn }, item);
    }

    // PUT: api/DeceasedPeople/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, DeceasedPerson item)
    {
        if (id != item.Ssn) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.DeceasedPeople.AnyAsync(e => e.Ssn == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/DeceasedPeople/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.DeceasedPeople.FindAsync(id);
        if (item == null) return NotFound();
        _context.DeceasedPeople.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
