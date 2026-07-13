using Microsoft.AspNetCore.Authorization;
using Graveyard.API.Data;
using Graveyard.API.Dtos;
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

    // ---- Birlesik defin islemleri (kisi + vefat tek adimda) ----

    // GET: api/DeceasedPeople/recent -> son vefat edenler (halka acik, sadece ad+konum+tarih)
    [AllowAnonymous]
    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<RecentDeathDto>>> Recent()
        => await _context.DeceasedPeople
            .Where(d => d.DateOfDeath != null)
            .OrderByDescending(d => d.DateOfDeath)
            .Take(12)
            .Select(d => new RecentDeathDto(
                d.SsnNavigation.FirstName + " " + d.SsnNavigation.LastName,
                d.DateOfDeath,
                d.PlotNumberNavigation != null ? d.PlotNumberNavigation.Zone!.Name : null,
                d.PlotNumber))
            .ToListAsync();

    // GET: api/DeceasedPeople/full/{ssn} -> duzenleme icin birlesik veri
    [HttpGet("full/{ssn}")]
    public async Task<ActionResult<CreateBurialDto>> GetFull(string ssn)
    {
        var dec = await _context.DeceasedPeople
            .Include(d => d.SsnNavigation)
            .FirstOrDefaultAsync(d => d.Ssn == ssn);
        if (dec == null) return NotFound();
        var p = dec.SsnNavigation;
        return new CreateBurialDto(dec.Ssn, p?.FirstName, p?.LastName, p?.DateOfBirth, p?.Gender,
            p?.MotherName, p?.FatherName,
            dec.DateOfDeath, dec.BurialDate, dec.CauseOfDeath, dec.Religion, dec.VeteranStatus,
            dec.FuneralPreferences, dec.PlotNumber, dec.PermitNumber);
    }

    // POST: api/DeceasedPeople/full -> kisiyi olustur/guncelle + vefat kaydi ekle
    [Authorize(Roles = "Admin")]
    [HttpPost("full")]
    public async Task<IActionResult> CreateFull(CreateBurialDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Ssn)) return BadRequest("SSN zorunlu.");

        var person = await _context.People.FindAsync(dto.Ssn);
        if (person == null)
        {
            person = new Person { Ssn = dto.Ssn };
            _context.People.Add(person);
        }
        person.FirstName = dto.FirstName;
        person.LastName = dto.LastName;
        person.DateOfBirth = dto.DateOfBirth;
        person.Gender = dto.Gender;
        person.MotherName = dto.MotherName;
        person.FatherName = dto.FatherName;

        if (await _context.DeceasedPeople.AnyAsync(d => d.Ssn == dto.Ssn))
            return Conflict("Bu kisi zaten vefat kaydinda.");

        _context.DeceasedPeople.Add(new DeceasedPerson
        {
            Ssn = dto.Ssn,
            DateOfDeath = dto.DateOfDeath,
            BurialDate = dto.BurialDate,
            CauseOfDeath = dto.CauseOfDeath,
            Religion = dto.Religion,
            VeteranStatus = dto.VeteranStatus,
            FuneralPreferences = dto.FuneralPreferences,
            PlotNumber = string.IsNullOrWhiteSpace(dto.PlotNumber) ? null : dto.PlotNumber,
            PermitNumber = string.IsNullOrWhiteSpace(dto.PermitNumber) ? null : dto.PermitNumber,
        });

        await _context.SaveChangesAsync(); // tek transaction
        return Ok();
    }

    // PUT: api/DeceasedPeople/full/{ssn} -> kisi + vefat bilgilerini guncelle
    [Authorize(Roles = "Admin")]
    [HttpPut("full/{ssn}")]
    public async Task<IActionResult> UpdateFull(string ssn, CreateBurialDto dto)
    {
        var person = await _context.People.FindAsync(ssn);
        if (person != null)
        {
            person.FirstName = dto.FirstName;
            person.LastName = dto.LastName;
            person.DateOfBirth = dto.DateOfBirth;
            person.Gender = dto.Gender;
            person.MotherName = dto.MotherName;
            person.FatherName = dto.FatherName;
        }

        var dec = await _context.DeceasedPeople.FindAsync(ssn);
        if (dec == null) return NotFound();
        dec.DateOfDeath = dto.DateOfDeath;
        dec.BurialDate = dto.BurialDate;
        dec.CauseOfDeath = dto.CauseOfDeath;
        dec.Religion = dto.Religion;
        dec.VeteranStatus = dto.VeteranStatus;
        dec.FuneralPreferences = dto.FuneralPreferences;
        dec.PlotNumber = string.IsNullOrWhiteSpace(dto.PlotNumber) ? null : dto.PlotNumber;
        dec.PermitNumber = string.IsNullOrWhiteSpace(dto.PermitNumber) ? null : dto.PermitNumber;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/DeceasedPeople/full/{ssn} -> vefat kaydini sil (kisi kalir)
    [Authorize(Roles = "Admin")]
    [HttpDelete("full/{ssn}")]
    public async Task<IActionResult> DeleteFull(string ssn)
    {
        var dec = await _context.DeceasedPeople.FindAsync(ssn);
        if (dec == null) return NotFound();
        _context.DeceasedPeople.Remove(dec);
        await _context.SaveChangesAsync();
        return NoContent();
    }

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
