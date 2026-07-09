using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public PaymentsController(GraveyardDbContext context) => _context = context;

    // GET: api/Payments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetAll()
        => await _context.Payments.ToListAsync();

    // GET: api/Payments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetById(string id)
    {
        var item = await _context.Payments.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/Payments
    [HttpPost]
    public async Task<ActionResult<Payment>> Create(Payment item)
    {
        _context.Payments.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.ReceiptNo }, item);
    }

    // PUT: api/Payments/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Payment item)
    {
        if (id != item.ReceiptNo) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Payments.AnyAsync(e => e.ReceiptNo == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/Payments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.Payments.FindAsync(id);
        if (item == null) return NotFound();
        _context.Payments.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
