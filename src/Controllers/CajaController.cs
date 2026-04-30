using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class CajaController(ICajaRepository repo, IEmpleadoRepository empleadoRepo) : BaseController
{
    [HttpGet("estado")]
    public async Task<IActionResult> Estado()
    {
        return ApiOk(await repo.ObtenerEstadoActualAsync());
    }

    [HttpGet("historial")]
    public async Task<IActionResult> Historial(int page = 1, int pageSize = 20)
    {
        var ps = Math.Clamp(pageSize, 1, 100);
        return ApiOk(await repo.ObtenerHistorialAsync(page, ps));
    }

    [HttpPost("abrir")]
    public async Task<IActionResult> Abrir([FromBody] AbrirCajaRequest req)
    {
        if (await repo.ObtenerAperturaActualAsync() is not null)
            throw new InvalidOperationException("Ya existe una caja abierta.");

        var responsableIds = (req.ResponsableIds ?? []).Distinct().ToList();
        if (responsableIds.Count == 0)
            throw new ArgumentException("Debe seleccionar al menos un responsable para abrir caja.");

        foreach (var empleadoId in responsableIds)
        {
            var empleado = await empleadoRepo.ObtenerPorIdAsync(empleadoId);
            if (empleado is null || !empleado.Activo)
                throw new ArgumentException($"El responsable con ID {empleadoId} no existe o está inactivo.");
        }

        var apertura = new CajaApertura
        {
            MontoInicial = req.MontoInicial,
            Responsables = responsableIds
                .Select(id => new CajaAperturaResponsable { EmpleadoId = id })
                .ToList()
        };
        return ApiCreated(await repo.AbrirAsync(apertura));
    }

    [HttpGet("{id:int}/movimientos")]
    public async Task<IActionResult> Movimientos(int id) =>
        ApiOk(await repo.ObtenerMovimientosAsync(id));

    [HttpPost("cerrar/{id:int}")]
    public async Task<IActionResult> Cerrar(int id, [FromBody] CerrarCajaRequest req)
    {
        var cierre = await repo.CerrarAsync(id, req.Observaciones)
            ?? throw new KeyNotFoundException("No se encontró una caja abierta con ese ID.");
        return ApiOk(cierre);
    }
}
