using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OrigenDashboard.Data.Seeders;
using OrigenDashboard.Repositories.Implementations;
using OrigenDashboard.Repositories.Interfaces;
using OrigenDashboard.Services;
using OrigenDashboard.Settings;

namespace OrigenDashboard.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
        services.AddScoped<IServicioRepository, ServicioRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IPaqueteRepository, PaqueteRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IIngresoRepository, IngresoRepository>();
        services.AddScoped<IEgresoRepository, EgresoRepository>();
        services.AddScoped<ICajaRepository, CajaRepository>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IInitialDataSeeder, InitialDataSeeder>();
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = configuration.GetSection(JwtOptions.Section).Get<JwtOptions>()
                    ?? throw new InvalidOperationException("Jwt no configurado.");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(AppConstants.Cors.PolicyName, policy =>
            {
                var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                    ?? throw new InvalidOperationException("Cors:AllowedOrigins no configurado.");

                policy.WithOrigins(origins)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });
        return services;
    }
}
