using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Endpoints;

public static class CajaEndpoints
{
    public static void MapCajaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/caja")
            .WithTags("Caja")
            .RequireAuthorization();

        group.MapGet("/estado", async (ICajaRepository repo) =>
        {
            var apertura = await repo.ObtenerAperturaActualAsync();
            return Results.Json(new { ok = true, data = apertura });
        })
        .WithName("EstadoCaja")
        .WithSummary("Devuelve la apertura de caja actualmente abierta, o null si está cerrada");

        group.MapGet("/historial", async (ICajaRepository repo, int page = 1, int pageSize = 20) =>
        {
            var ps = Math.Clamp(pageSize, 1, 100);
            var result = await repo.ObtenerHistorialAsync(page, ps);
            return Results.Json(new { ok = true, data = result });
        })
        .WithName("HistorialCaja")
        .WithSummary("Lista el historial de cierres de caja paginado");

        group.MapPost("/abrir", async (AbrirCajaRequest req, ICajaRepository repo) =>
        {
            var actual = await repo.ObtenerAperturaActualAsync();
            if (actual is not null)
                return Results.Json(new { error = "Ya existe una caja abierta." }, statusCode: 409);

            var apertura = new CajaApertura
            {
                MontoInicial = req.MontoInicial,
                Responsables = req.Responsables
            };
            var creada = await repo.AbrirAsync(apertura);
            return Results.Json(new { ok = true, data = creada }, statusCode: 201);
        })
        .WithName("AbrirCaja")
        .WithSummary("Abre una nueva sesión de caja");

        group.MapPost("/cerrar/{id:int}", async (int id, CerrarCajaRequest req, ICajaRepository repo) =>
        {
            var ok = await repo.CerrarAsync(id, req.TotalIngresos, req.TotalEgresos, req.SaldoFinal, req.Observaciones);
            return ok
                ? Results.Json(new { ok = true })
                : Results.Json(new { error = "No se encontró una caja abierta con ese ID." }, statusCode: 404);
        })
        .WithName("CerrarCaja")
        .WithSummary("Cierra la sesión de caja activa");
    }
}
