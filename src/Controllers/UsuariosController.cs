using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

[Authorize(Roles = AppConstants.Roles.Admin)]
public class UsuariosController(IUsuarioRepository repo) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Listar(int? page, int? pageSize)
    {
        if (page.HasValue && pageSize.HasValue)
        {
            var ps = Math.Clamp(pageSize.Value, 1, 100);
            var result = await repo.ObtenerPaginadoAsync(page.Value, ps);
            return ApiOk(new
            {
                result.Total,
                result.Page,
                result.PageSize,
                Items = result.Items.Select(u => new { u.Id, u.NombreUsuario, u.NombreCompleto, u.Rol, u.Activo })
            });
        }
        var lista = await repo.ObtenerTodosAsync();
        return ApiOk(lista.Select(u => new { u.Id, u.NombreUsuario, u.NombreCompleto, u.Rol, u.Activo }));
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioRequest req)
    {
        if (await repo.ObtenerPorUsuarioAsync(req.NombreUsuario) is not null)
            throw new InvalidOperationException("El nombre de usuario ya existe.");

        var usuario = new Usuario
        {
            NombreCompleto = req.NombreCompleto,
            NombreUsuario = req.NombreUsuario,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Rol = req.Rol
        };

        var creado = await repo.CrearAsync(usuario);
        return ApiCreated(new { creado.Id, creado.NombreUsuario, creado.NombreCompleto, creado.Rol });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioRequest req)
    {
        var usuario = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        usuario.NombreCompleto = req.NombreCompleto;
        usuario.Rol = req.Rol;
        usuario.Activo = req.Activo;

        if (!string.IsNullOrWhiteSpace(req.Password))
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        await repo.ActualizarAsync(usuario);
        return ApiOk();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!await repo.EliminarAsync(id))
            throw new KeyNotFoundException("Usuario no encontrado.");
        return ApiOk();
    }
}
