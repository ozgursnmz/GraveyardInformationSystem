using System.Text;
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

// CORS (frontend baglanacak)
builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

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
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsJsonAsync(new
        {
            error = dbEx.InnerException?.Message ?? "Veritabanı işlemi başarısız.",
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

// Statik frontend (wwwroot): acilis sayfasi = halka acik mezar sorgulama (find.html)
var defaultFiles = new DefaultFilesOptions();
defaultFiles.DefaultFileNames.Clear();
defaultFiles.DefaultFileNames.Add("find.html");
app.UseDefaultFiles(defaultFiles);
app.UseStaticFiles();

app.UseCors("AllowAll");
app.UseAuthentication();   // once kimlik dogrula
app.UseAuthorization();    // sonra yetki kontrol et
app.MapControllers();

app.Run();
