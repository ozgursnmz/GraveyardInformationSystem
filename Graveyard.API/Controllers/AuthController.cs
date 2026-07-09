using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Graveyard.API.Data;
using Graveyard.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Graveyard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GraveyardDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(GraveyardDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // POST: api/Auth/login  -> kullanici adi + sifre ile JWT token al (halka acik)
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _context.AppUsers
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        // Kullanici yok VEYA sifre yanlis -> ayni mesaj (guvenlik)
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Kullanici adi veya sifre hatali.");

        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponse(tokenString, user.Username, user.Role, expires);
    }
}
