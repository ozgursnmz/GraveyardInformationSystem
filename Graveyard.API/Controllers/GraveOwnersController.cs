using Microsoft.AspNetCore.Authorization;
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

    // GET: api/GraveOwners?archived=false  -> varsayilan: aktif kayitlar
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GraveOwner>>> GetAll(bool archived = false)
        => await _context.GraveOwners.Where(o => o.IsArchived == archived).ToListAsync();

    // GET: api/GraveOwners/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<GraveOwner>> GetById(string id)
    {
        var item = await _context.GraveOwners.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/GraveOwners
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<GraveOwner>> Create(GraveOwner item)
    {
        _context.GraveOwners.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Ssn }, item);
    }

    // PUT: api/GraveOwners/{id}
    [Authorize(Roles = "Admin")]
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

    // DELETE: api/GraveOwners/{id}  -> kalici silmez, arsivler (soft delete)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.GraveOwners.FindAsync(id);
        if (item == null) return NotFound();
        item.IsArchived = true;
        item.ArchivedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // POST: api/GraveOwners/{id}/restore  -> arsivden geri getir
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(string id)
    {
        var item = await _context.GraveOwners.FindAsync(id);
        if (item == null) return NotFound();
        item.IsArchived = false;
        item.ArchivedAt = null;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/GraveOwners/{id}/permanent  -> KALICI sil (yalnizca arsivdeki kayit)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}/permanent")]
    public async Task<IActionResult> PurgePermanent(string id)
    {
        var item = await _context.GraveOwners.FindAsync(id);
        if (item == null) return NotFound();
        if (!item.IsArchived) return BadRequest("Sadece arşivdeki kayıtlar kalıcı silinebilir.");
        _context.GraveOwners.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
