using Graveyard.API.Data;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditLogsController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public AuditLogsController(GraveyardDbContext context) => _context = context;

    // GET: api/AuditLogs -> son islem kayitlari (en yeni ustte)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLog>>> GetAll()
        => await _context.AuditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(500)
            .ToListAsync();
}
