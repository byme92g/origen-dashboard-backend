using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrigenDashboard.Data;
using OrigenDashboard.Data.Seeders;
using OrigenDashboard.Endpoints;
using OrigenDashboard.Repositories.Implementations;
using OrigenDashboard.Repositories.Interfaces;
using OrigenDashboard.Services;
using OrigenDashboard.Settings;

var builder = WebApplication.CreateBuilder(args);

// ── OpenAPI ───────────────────────────────────────────────────────────────
builder.Services.AddOpenApi();

// ── JSON: evitar ciclos de referencia de EF Core ──────────────────────────
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// ── MySQL / EF Core ───────────────────────────────────────────────────────
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

// ── JWT Authentication ────────────────────────────────────────────────────
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret no configurado.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero // sin margen extra — el token expira exactamente a los 30 min
        };
    });

builder.Services.AddAuthorization();

// ── CORS (Blazor WASM) ────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:5173", "https://localhost:7173"];

        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Repositorios ──────────────────────────────────────────────────────────
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IPaqueteRepository, PaqueteRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IIngresoRepository, IngresoRepository>();
builder.Services.AddScoped<IEgresoRepository, EgresoRepository>();

// ── Servicios ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IInitialDataSeeder, InitialDataSeeder>();

// ── Admin options ─────────────────────────────────────────────────────────
builder.Services.AddOptions<AdminOptions>()
    .BindConfiguration("")
    .ValidateOnStart();

// ── Build ─────────────────────────────────────────────────────────────────
var app = builder.Build();

app.ValidateConfiguration();

// ── Middleware ────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseCors("BlazorPolicy");
app.UseAuthentication();
app.UseAuthorization();

// ── Health check ─────────────────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

// ── Endpoints ─────────────────────────────────────────────────────────────
app.MapAuthEndpoints();
app.MapUsuariosEndpoints();
app.MapEmpleadosEndpoints();
app.MapClientesEndpoints();
app.MapServiciosEndpoints();
app.MapProductosEndpoints();
app.MapPaquetesEndpoints();
app.MapIngresosEndpoints();
app.MapEgresosEndpoints();
app.MapReportesEndpoints();

// ── Migración automática al iniciar (con retry para Docker) ──────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
    var retries = 5;
    while (retries-- > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (Exception ex) when (retries > 0)
        {
            logger.LogWarning("Esperando a MySQL... {Error}. Reintentos restantes: {Retries}", ex.Message, retries);
            await Task.Delay(3000);
        }
    }

    // ── Seed: admin inicial ───────────────────────────────────────────────
    var adminOpts = scope.ServiceProvider.GetRequiredService<IOptions<AdminOptions>>().Value;
    var adminUser = adminOpts.AdminUsername;
    var adminPass = adminOpts.AdminPassword;

    if (!db.Usuarios.Any(u => u.Rol == "admin"))
    {
        db.Usuarios.Add(new OrigenDashboard.Models.Entities.Usuario
        {
            NombreUsuario  = adminUser,
            NombreCompleto = "Administrador",
            PasswordHash   = BCrypt.Net.BCrypt.HashPassword(adminPass),
            Rol            = "admin",
            Activo         = true
        });
        await db.SaveChangesAsync();
        logger.LogInformation("Usuario admin creado: {Usuario}", adminUser);
    }

    // ── Seed: datos de prueba ─────────────────────────────────────────────
    if (app.Environment.IsDevelopment() || args.Contains("--seed"))
    {
        var seeder = scope.ServiceProvider.GetRequiredService<IInitialDataSeeder>();
        await seeder.SeedAsync(db);
        logger.LogInformation("Datos de prueba cargados exitosamente");
    }
}

app.Run();
