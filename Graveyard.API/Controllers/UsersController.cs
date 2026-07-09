using Graveyard.API.Data;
using Graveyard.API.Dtos;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]   // tum kullanici yonetimi sadece admin
public class UsersController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    public UsersController(GraveyardDbContext context) => _context = context;

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        => await _context.AppUsers
            .Select(u => new UserDto(u.UserId, u.Username, u.Role, u.CreatedAt))
            .ToListAsync();

    // GET: api/Users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var u = await _context.AppUsers.FindAsync(id);
        if (u == null) return NotFound();
        return new UserDto(u.UserId, u.Username, u.Role, u.CreatedAt);
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Kullanici adi ve sifre zorunlu.");
        if (await _context.AppUsers.AnyAsync(u => u.Username == dto.Username))
            return Conflict("Bu kullanici adi zaten kayitli.");

        var user = new AppUser
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = string.IsNullOrWhiteSpace(dto.Role) ? "Admin" : dto.Role,
            CreatedAt = DateTime.Now,
        };
        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.UserId },
            new UserDto(user.UserId, user.Username, user.Role, user.CreatedAt));
    }

    // PUT: api/Users/{id}  (rol ve/veya sifre guncelle)
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateUserDto dto)
    {
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Role)) user.Role = dto.Role;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        // Son admini silmeyi engelle (sisteme kilitlenmeyi onler)
        if (user.Role == "Admin" && await _context.AppUsers.CountAsync(u => u.Role == "Admin") <= 1)
            return BadRequest("Son admin kullanicisi silinemez.");

        _context.AppUsers.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
