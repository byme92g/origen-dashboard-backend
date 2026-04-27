using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class CajaController(ICajaRepository repo) : BaseController
{
    [HttpGet("estado")]
    public async Task<IActionResult> Estado()
    {
        var apertura = await repo.ObtenerAperturaActualAsync();
        return ApiOk(apertura);
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

        var apertura = new CajaApertura
        {
            MontoInicial = req.MontoInicial,
            Responsables = req.Responsables
        };
        return ApiCreated(await repo.AbrirAsync(apertura));
    }

    [HttpPost("cerrar/{id:int}")]
    public async Task<IActionResult> Cerrar(int id, [FromBody] CerrarCajaRequest req)
    {
        if (!await repo.CerrarAsync(id, req.TotalIngresos, req.TotalEgresos, req.SaldoFinal, req.Observaciones))
            throw new KeyNotFoundException("No se encontró una caja abierta con ese ID.");
        return ApiOk();
    }
}
