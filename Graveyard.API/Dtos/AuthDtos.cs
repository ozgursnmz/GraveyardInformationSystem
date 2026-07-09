namespace Graveyard.API.Dtos;

// Giris istegi (kullanici adi + sifre)
public record LoginRequest(string Username, string Password);

// Giris yaniti (token + kullanici bilgisi)
public record LoginResponse(string Token, string Username, string Role, DateTime ExpiresAt);
