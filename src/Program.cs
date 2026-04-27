using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using OrigenDashboard;
using OrigenDashboard.Data;
using OrigenDashboard.Data.Seeders;
using OrigenDashboard.Extensions;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Models.Enums;
using OrigenDashboard.Settings;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi();

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
    });

// MySQL / EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 44)))
    .ConfigureWarnings(warnings =>
        warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

// JWT Authentication + Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS
builder.Services.AddFrontendCors(builder.Configuration);

// Repositories
builder.Services.AddRepositories();

// Services
builder.Services.AddApplicationServices();

// Admin options
builder.Services.AddOptions<AdminOptions>()
    .BindConfiguration("")
    .ValidateOnStart();

var app = builder.Build();

// Startup validation
app.ValidateConfiguration();

// Middleware
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseCors(AppConstants.Cors.PolicyName);
app.UseAuthentication();
app.UseAuthorization();

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

// Controllers
app.MapControllers();

// Auto-migration con retry para Docker
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

    if (args.Any(a => a.Equals(AppConstants.Args.CreateAdmin, StringComparison.OrdinalIgnoreCase)))
    {
        var adminOpts = scope.ServiceProvider.GetRequiredService<IOptions<AdminOptions>>().Value;
        var existing = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == adminOpts.AdminUsername);
        if (existing is not null)
        {
            existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminOpts.AdminPassword);
            existing.Activo = true;
        }
        else
        {
            db.Usuarios.Add(new Usuario
            {
                NombreUsuario  = adminOpts.AdminUsername,
                NombreCompleto = "Administrador",
                PasswordHash   = BCrypt.Net.BCrypt.HashPassword(adminOpts.AdminPassword),
                Rol            = RolUsuario.Admin,
                Activo         = true
            });
        }
        await db.SaveChangesAsync();
        logger.LogInformation("Admin '{User}' actualizado/creado via {Flag}", adminOpts.AdminUsername, AppConstants.Args.CreateAdmin);
    }

    if (app.Environment.IsDevelopment() || args.Contains(AppConstants.Args.Seed))
    {
        var seeder = scope.ServiceProvider.GetRequiredService<IInitialDataSeeder>();
        await seeder.SeedAsync(db);
        logger.LogInformation("Datos de prueba cargados exitosamente");
    }
}

app.Run();
