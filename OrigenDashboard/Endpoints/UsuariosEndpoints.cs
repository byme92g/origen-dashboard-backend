using Microsoft.AspNetCore.Authorization;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Endpoints;

public static class UsuariosEndpoints
{
    public static void MapUsuariosEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/usuarios")
            .WithTags("Usuarios")
            .RequireAuthorization();

        group.MapGet("/", async (IUsuarioRepository repo) =>
        {
            var lista = await repo.ObtenerTodosAsync();
            return Results.Json(new { ok = true, data = lista.Select(u => new { u.Id, u.NombreUsuario, u.NombreCompleto, u.Rol, u.Activo }) });
        })
        .WithName("ListarUsuarios")
        .WithSummary("Lista todos los usuarios (solo admin)");

        group.MapPost("/", async (
            CrearUsuarioRequest req,
            IUsuarioRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(req.NombreCompleto) || string.IsNullOrWhiteSpace(req.NombreUsuario) || string.IsNullOrWhiteSpace(req.Password))
                return Results.Json(new { error = "Nombre, usuario y contraseña son requeridos." }, statusCode: 400);

            if (await repo.ObtenerPorUsuarioAsync(req.NombreUsuario) is not null)
                return Results.Json(new { error = "El nombre de usuario ya existe." }, statusCode: 409);

            var usuario = new Usuario
            {
                NombreCompleto = req.NombreCompleto,
                NombreUsuario = req.NombreUsuario,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Rol = req.Rol
            };

            var creado = await repo.CrearAsync(usuario);
            return Results.Json(new { ok = true, data = new { creado.Id, creado.NombreUsuario, creado.NombreCompleto, creado.Rol } }, statusCode: 201);
        })
        .WithName("CrearUsuario")
        .WithSummary("Crea un nuevo usuario");

        group.MapPut("/{id:int}", async (
            int id,
            ActualizarUsuarioRequest req,
            IUsuarioRepository repo) =>
        {
            var usuario = await repo.ObtenerPorIdAsync(id);
            if (usuario is null)
                return Results.Json(new { error = "Usuario no encontrado." }, statusCode: 404);

            usuario.NombreCompleto = req.NombreCompleto;
            usuario.Rol = req.Rol;
            usuario.Activo = req.Activo;

            if (!string.IsNullOrWhiteSpace(req.Password))
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            await repo.ActualizarAsync(usuario);
            return Results.Json(new { ok = true });
        })
        .WithName("ActualizarUsuario")
        .WithSummary("Actualiza un usuario");

        group.MapDelete("/{id:int}", async (int id, IUsuarioRepository repo) =>
        {
            var ok = await repo.EliminarAsync(id);
            return ok
                ? Results.Json(new { ok = true })
                : Results.Json(new { error = "Usuario no encontrado." }, statusCode: 404);
        })
        .WithName("EliminarUsuario")
        .WithSummary("Elimina un usuario");
    }
}
