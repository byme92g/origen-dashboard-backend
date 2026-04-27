using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class ServiciosController(IServicioRepository repo) : BaseController
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
        var servicio = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Servicio no encontrado.");
        return ApiOk(servicio);
    }

    [HttpPost]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Crear([FromBody] CrearServicioRequest req)
    {
        var servicio = new Servicio
        {
            Nombre = req.Nombre,
            Categoria = req.Categoria,
            Precio = req.Precio,
            DuracionMin = req.DuracionMin,
            ComisionPct = req.ComisionPct
        };
        return ApiCreated(await repo.CrearAsync(servicio));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarServicioRequest req)
    {
        var servicio = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Servicio no encontrado.");

        servicio.Nombre = req.Nombre;
        servicio.Categoria = req.Categoria;
        servicio.Precio = req.Precio;
        servicio.DuracionMin = req.DuracionMin;
        servicio.ComisionPct = req.ComisionPct;
        servicio.Activo = req.Activo;

        await repo.ActualizarAsync(servicio);
        return ApiOk(servicio);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!await repo.EliminarAsync(id))
            throw new KeyNotFoundException("Servicio no encontrado.");
        return ApiOk();
    }
}
