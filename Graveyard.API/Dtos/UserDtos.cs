namespace Graveyard.API.Dtos;

// Kullanici listeleme (sifre hash'i asla donmez)
public record UserDto(int UserId, string Username, string Role, DateTime CreatedAt);

// Yeni kullanici
public record CreateUserDto(string Username, string Password, string? Role);

// Guncelleme (rol ve/veya sifre sifirlama; bos birakilanlar degismez)
public record UpdateUserDto(string? Role, string? Password);
