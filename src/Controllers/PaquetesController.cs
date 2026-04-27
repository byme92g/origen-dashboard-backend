using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class PaquetesController(IPaqueteRepository repo) : BaseController
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
        var paquete = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Paquete no encontrado.");
        return ApiOk(paquete);
    }

    [HttpPost]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Crear([FromBody] CrearPaqueteRequest req)
    {
        var paquete = new Paquete
        {
            Nombre = req.Nombre,
            Descripcion = req.Descripcion,
            Precio = req.Precio,
            Descuento = req.Descuento,
            ComisionPct = req.ComisionPct
        };
        return ApiCreated(await repo.CrearAsync(paquete, req.ServicioIds, req.ProductoIds));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarPaqueteRequest req)
    {
        var paquete = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Paquete no encontrado.");

        paquete.Nombre = req.Nombre;
        paquete.Descripcion = req.Descripcion;
        paquete.Precio = req.Precio;
        paquete.Descuento = req.Descuento;
        paquete.ComisionPct = req.ComisionPct;
        paquete.Activo = req.Activo;

        await repo.ActualizarAsync(paquete, req.ServicioIds, req.ProductoIds);
        return ApiOk(paquete);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!await repo.EliminarAsync(id))
            throw new KeyNotFoundException("Paquete no encontrado.");
        return ApiOk();
    }
}
