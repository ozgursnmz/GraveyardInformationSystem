using Graveyard.API.Data;
using Graveyard.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BurialRecordsController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public BurialRecordsController(GraveyardDbContext context) => _context = context;

    // Ortak projeksiyon: entity zincirini DTO'ya cevirir (EF otomatik LEFT JOIN uretir)
    private IQueryable<BurialRecordDto> BaseQuery() =>
        _context.DeceasedPeople.Select(d => new BurialRecordDto(
            d.Ssn,
            d.SsnNavigation.FirstName + " " + d.SsnNavigation.LastName,
            d.SsnNavigation.DateOfBirth,
            d.DateOfDeath,
            d.CauseOfDeath,
            d.Religion,
            d.PlotNumber,
            d.PlotNumberNavigation!.Status,
            d.PlotNumberNavigation.Zone!.Name,
            d.PlotNumberNavigation.MonumentCodeNavigation!.Material + " " +
                d.PlotNumberNavigation.MonumentCodeNavigation.Style,
            d.PlotNumberNavigation.Reservation!.OwnerSsnNavigation!.SsnNavigation.FirstName + " " +
                d.PlotNumberNavigation.Reservation.OwnerSsnNavigation.SsnNavigation.LastName,
            d.PlotNumberNavigation.Reservation.OwnerSsnNavigation.PhoneNumber,
            d.PlotNumberNavigation.Reservation.OwnerSsnNavigation.Email,
            d.PermitNumber
        ));

    // GET: api/BurialRecords?archived=false  -> defin kayitlari (varsayilan: aktif)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BurialRecordDto>>> GetAll(bool archived = false)
        => await _context.DeceasedPeople
            .Where(d => d.IsArchived == archived)
            .Select(d => new BurialRecordDto(
                d.Ssn,
                d.SsnNavigation.FirstName + " " + d.SsnNavigation.LastName,
                d.SsnNavigation.DateOfBirth,
                d.DateOfDeath,
                d.CauseOfDeath,
                d.Religion,
                d.PlotNumber,
                d.PlotNumberNavigation!.Status,
                d.PlotNumberNavigation.Zone!.Name,
                d.PlotNumberNavigation.MonumentCodeNavigation!.Material + " " +
                    d.PlotNumberNavigation.MonumentCodeNavigation.Style,
                d.PlotNumberNavigation.Reservation!.OwnerSsnNavigation!.SsnNavigation.FirstName + " " +
                    d.PlotNumberNavigation.Reservation.OwnerSsnNavigation.SsnNavigation.LastName,
                d.PlotNumberNavigation.Reservation.OwnerSsnNavigation.PhoneNumber,
                d.PlotNumberNavigation.Reservation.OwnerSsnNavigation.Email,
                d.PermitNumber))
            .ToListAsync();

    // GET: api/BurialRecords/10000000001  -> tek defin kaydi
    [HttpGet("{ssn}")]
    public async Task<ActionResult<BurialRecordDto>> GetById(string ssn)
    {
        var record = await BaseQuery().FirstOrDefaultAsync(r => r.Ssn == ssn);
        if (record == null) return NotFound();
        return record;
    }
}
