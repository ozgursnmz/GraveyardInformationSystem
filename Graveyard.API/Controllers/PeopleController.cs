using Microsoft.AspNetCore.Authorization;
using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public PeopleController(GraveyardDbContext context) => _context = context;

    // GET: api/People?archived=false  -> varsayilan: aktif (arsivlenmemis) kayitlar
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Person>>> GetAll(bool archived = false)
        => await _context.People.Where(p => p.IsArchived == archived).ToListAsync();

    // GET: api/People/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Person>> GetById(string id)
    {
        var item = await _context.People.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // GET: api/People/search?name=ahmet&gender=Male  -> ad/soyad akilli arama
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Person>>> Search(string? name, string? gender)
    {
        var query = _context.People.AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => (p.FirstName + " " + p.LastName).Contains(name));
        if (!string.IsNullOrWhiteSpace(gender))
            query = query.Where(p => p.Gender == gender);
        return await query.ToListAsync();
    }

    // POST: api/People
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Person>> Create(Person item)
    {
        _context.People.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Ssn }, item);
    }

    // PUT: api/People/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Person item)
    {
        if (id != item.Ssn) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.People.AnyAsync(e => e.Ssn == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/People/{id}  -> kalici silmez, arsivler (soft delete)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.People.FindAsync(id);
        if (item == null) return NotFound();
        item.IsArchived = true;
        item.ArchivedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // POST: api/People/{id}/restore  -> arsivden geri getir
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(string id)
    {
        var item = await _context.People.FindAsync(id);
        if (item == null) return NotFound();
        item.IsArchived = false;
        item.ArchivedAt = null;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/People/{id}/permanent  -> KALICI sil (yalnizca arsivdeki kayit)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}/permanent")]
    public async Task<IActionResult> PurgePermanent(string id)
    {
        var item = await _context.People.FindAsync(id);
        if (item == null) return NotFound();
        if (!item.IsArchived) return BadRequest("Sadece arşivdeki kayıtlar kalıcı silinebilir.");
        _context.People.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
