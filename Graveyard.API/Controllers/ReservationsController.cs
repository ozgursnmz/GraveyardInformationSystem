using Microsoft.AspNetCore.Authorization;
using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public ReservationsController(GraveyardDbContext context) => _context = context;

    // GET: api/Reservations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reservation>>> GetAll()
        => await _context.Reservations.ToListAsync();

    // GET: api/Reservations/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> GetById(string id)
    {
        var item = await _context.Reservations.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/Reservations
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Reservation>> Create(Reservation item)
    {
        _context.Reservations.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.ReservationId }, item);
    }

    // PUT: api/Reservations/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Reservation item)
    {
        if (id != item.ReservationId) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Reservations.AnyAsync(e => e.ReservationId == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/Reservations/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.Reservations.FindAsync(id);
        if (item == null) return NotFound();
        _context.Reservations.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
