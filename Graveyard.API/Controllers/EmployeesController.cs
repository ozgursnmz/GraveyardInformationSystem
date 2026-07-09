using Microsoft.AspNetCore.Authorization;
using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public EmployeesController(GraveyardDbContext context) => _context = context;

    // GET: api/Employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
        => await _context.Employees.ToListAsync();

    // GET: api/Employees/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetById(string id)
    {
        var item = await _context.Employees.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    // POST: api/Employees
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Employee>> Create(Employee item)
    {
        _context.Employees.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.EmployeeId }, item);
    }

    // PUT: api/Employees/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Employee item)
    {
        if (id != item.EmployeeId) return BadRequest("URL id ile govde anahtari uyusmuyor.");
        _context.Entry(item).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Employees.AnyAsync(e => e.EmployeeId == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/Employees/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.Employees.FindAsync(id);
        if (item == null) return NotFound();
        _context.Employees.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
