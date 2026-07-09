using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceLogsController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public MaintenanceLogsController(GraveyardDbContext context) => _context = context;

    // GET: api/MaintenanceLogs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaintenanceLog>>> GetAll()
        => await _context.MaintenanceLogs.ToListAsync();

    // GET: api/MaintenanceLogs/PLT001/LOG001  (bilesik anahtar)
    [HttpGet("{plotNumber}/{logNo}")]
    public async Task<ActionResult<MaintenanceLog>> GetById(string plotNumber, string logNo)
    {
        var item = await _context.MaintenanceLogs.FindAsync(plotNumber, logNo);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/MaintenanceLogs
    [HttpPost]
    public async Task<ActionResult<MaintenanceLog>> Create(MaintenanceLog item)
    {
        _context.MaintenanceLogs.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById),
            new { plotNumber = item.PlotNumber, logNo = item.LogNo }, item);
    }

    // PUT: api/MaintenanceLogs/PLT001/LOG001
    [HttpPut("{plotNumber}/{logNo}")]
    public async Task<IActionResult> Update(string plotNumber, string logNo, MaintenanceLog item)
    {
        if (plotNumber != item.PlotNumber || logNo != item.LogNo)
            return BadRequest("URL anahtari ile govde anahtari uyusmuyor.");

        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.MaintenanceLogs.AnyAsync(e =>
                    e.PlotNumber == plotNumber && e.LogNo == logNo))
                return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/MaintenanceLogs/PLT001/LOG001
    [HttpDelete("{plotNumber}/{logNo}")]
    public async Task<IActionResult> Delete(string plotNumber, string logNo)
    {
        var item = await _context.MaintenanceLogs.FindAsync(plotNumber, logNo);
        if (item == null) return NotFound();
        _context.MaintenanceLogs.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
