using System.Text;
using System.Threading.RateLimiting;
using Graveyard.API.Data;
using Graveyard.API.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controller'lar
builder.Services.AddControllers(options => options.Filters.Add<AuditFilter>())
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// Veritabani baglantisi (appsettings.json'dan)
builder.Services.AddDbContext<GraveyardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT kimlik dogrulama
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });
// Varsayilan: TUM uclar kimlik dogrulamasi ister.
// Halka acik olmasi gerekenler [AllowAnonymous] ile isaretlenir (login + mezar sorgulama).
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Swagger (test arayuzu) + JWT token destegi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'i buraya yapistir (Bearer yazmana gerek yok)."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// NOT: Frontend (wwwroot) API ile AYNI kaynaktan sunuluyor; bu yuzden CORS'a gerek yok.
// (Onceki "AllowAnyOrigin" politikasi kaldirildi - gereksiz ve fazla genisti.)

// Giris denemelerine hiz siniri: kaba-kuvvet (brute force) saldirilarina karsi IP basina 5/dakika
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429; // Too Many Requests
    options.AddPolicy("login", ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1),
                PermitLimit = 5,
                QueueLimit = 0
            }));
});

var app = builder.Build();

// Global hata yonetimi: stack trace yerine temiz JSON dondur.
// DB kisit hatalarinda mesaji korur (frontend anlamli mesaja cevirir), digerlerinde genel mesaj.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DbUpdateException dbEx)
    {
        // Ham SQL mesajini (tablo/kisit adlari) istemciye SIZDIRMA; sadece kisa bir kod dondur.
        var inner = dbEx.InnerException?.Message ?? "";
        string code =
            inner.Contains("FOREIGN KEY") || inner.Contains("REFERENCE") ? "FK" :
            inner.Contains("PRIMARY KEY") || inner.Contains("duplicate") ? "DUPLICATE" :
            inner.Contains("CHECK constraint") ? "CHECK" :
            inner.Contains("NULL") ? "NULL" : "DB";
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Veritabanı işlemi başarısız.",
            code,
            status = 400
        });
    }
    catch (Exception)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Beklenmeyen bir hata oluştu.",
            status = 500
        });
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Uretimde HTTP'yi HTTPS'e yonlendir (token/sifre duz metin gitmesin).
    // Development'ta local http calismasi bozulmasin diye kapali.
    app.UseHttpsRedirection();
}

// Statik frontend (wwwroot): acilis sayfasi = halka acik mezar sorgulama (find.html)
var defaultFiles = new DefaultFilesOptions();
defaultFiles.DefaultFileNames.Clear();
defaultFiles.DefaultFileNames.Add("find.html");
app.UseDefaultFiles(defaultFiles);
app.UseStaticFiles();

app.UseRateLimiter();      // hiz siniri (login politikasi)
app.UseAuthentication();   // once kimlik dogrula
app.UseAuthorization();    // sonra yetki kontrol et
app.MapControllers();

app.Run();
