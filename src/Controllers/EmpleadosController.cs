using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class EmpleadosController(IEmpleadoRepository repo, IUsuarioRepository usuarioRepo) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Listar(int? page, int? pageSize)
    {
        if (page.HasValue && pageSize.HasValue)
        {
            var ps = Math.Clamp(pageSize.Value, 1, 100);
            return ApiOk(await repo.ObtenerPaginadoAsync(page.Value, ps));
        }
        return ApiOk(await repo.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var empleado = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Empleado no encontrado.");
        return ApiOk(empleado);
    }

    [HttpPost]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Crear([FromBody] CrearEmpleadoRequest req)
    {
        if (!string.IsNullOrWhiteSpace(req.UsuarioLogin) &&
            await usuarioRepo.ObtenerPorUsuarioAsync(req.UsuarioLogin) is null)
            throw new ArgumentException($"El usuario '{req.UsuarioLogin}' no existe.");

        var empleado = new Empleado
        {
            Nombre = req.Nombre,
            Cargo = req.Cargo,
            ComisionPct = req.ComisionPct,
            UsuarioLogin = req.UsuarioLogin
        };
        return ApiCreated(await repo.CrearAsync(empleado));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEmpleadoRequest req)
    {
        var empleado = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Empleado no encontrado.");

        if (!string.IsNullOrWhiteSpace(req.UsuarioLogin) &&
            await usuarioRepo.ObtenerPorUsuarioAsync(req.UsuarioLogin) is null)
            throw new ArgumentException($"El usuario '{req.UsuarioLogin}' no existe.");

        empleado.Nombre = req.Nombre;
        empleado.Cargo = req.Cargo;
        empleado.ComisionPct = req.ComisionPct;
        empleado.UsuarioLogin = req.UsuarioLogin;
        empleado.Activo = req.Activo;

        await repo.ActualizarAsync(empleado);
        return ApiOk(empleado);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!await repo.EliminarAsync(id))
            throw new KeyNotFoundException("Empleado no encontrado.");
        return ApiOk();
    }
}
