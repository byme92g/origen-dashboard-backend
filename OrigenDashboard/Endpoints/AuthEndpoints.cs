using OrigenDashboard.Models;
using OrigenDashboard.Repositories.Interfaces;
using OrigenDashboard.Services;

namespace OrigenDashboard.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/login", async (
            LoginRequest req,
            IUsuarioRepository usuarios,
            ITokenService tokenService) =>
        {
            if (string.IsNullOrWhiteSpace(req.NombreUsuario) || string.IsNullOrWhiteSpace(req.Password))
                return Results.Json(new { error = "Usuario y contraseña son requeridos." }, statusCode: 400);

            var usuario = await usuarios.ObtenerPorUsuarioAsync(req.NombreUsuario);
            if (usuario is null || !BCrypt.Net.BCrypt.Verify(req.Password, usuario.PasswordHash))
                return Results.Json(new { error = "Credenciales incorrectas." }, statusCode: 401);

            var token = tokenService.GenerarToken(usuario);

            return Results.Json(new
            {
                ok = true,
                token,
                expiraEn = 1800, // 30 minutos en segundos
                usuario = new
                {
                    id = usuario.Id,
                    nombreUsuario = usuario.NombreUsuario,
                    nombreCompleto = usuario.NombreCompleto,
                    rol = usuario.Rol
                }
            });
        })
        .WithName("Login")
        .WithSummary("Iniciar sesión y obtener token JWT (válido 30 minutos)")
        .AllowAnonymous();
    }
}
