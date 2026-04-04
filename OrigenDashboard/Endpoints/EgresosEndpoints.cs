using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Endpoints;

public static class EgresosEndpoints
{
    public static void MapEgresosEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/egresos")
            .WithTags("Egresos")
            .RequireAuthorization();

        group.MapGet("/", async (IEgresoRepository repo, DateTime? desde, DateTime? hasta) =>
        {
            var lista = desde.HasValue && hasta.HasValue
                ? await repo.ObtenerPorFechaAsync(desde.Value, hasta.Value)
                : await repo.ObtenerTodosAsync();

            return Results.Json(new { ok = true, data = lista });
        })
        .WithName("ListarEgresos")
        .WithSummary("Lista egresos, opcionalmente filtrados por rango de fecha");

        group.MapGet("/{id:int}", async (int id, IEgresoRepository repo) =>
        {
            var egreso = await repo.ObtenerPorIdAsync(id);
            return egreso is null
                ? Results.Json(new { error = "Egreso no encontrado." }, statusCode: 404)
                : Results.Json(new { ok = true, data = egreso });
        })
        .WithName("ObtenerEgreso")
        .WithSummary("Obtiene un egreso por ID");

        group.MapPost("/", async (CrearEgresoRequest req, IEgresoRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(req.Descripcion))
                return Results.Json(new { error = "La descripción es requerida." }, statusCode: 400);

            if (req.Monto <= 0)
                return Results.Json(new { error = "El monto debe ser mayor a 0." }, statusCode: 400);

            var egreso = new Egreso
            {
                Fecha = req.Fecha,
                Categoria = req.Categoria,
                Descripcion = req.Descripcion,
                Monto = req.Monto,
                Proveedor = req.Proveedor,
                Comprobante = req.Comprobante,
                Observaciones = req.Observaciones
            };

            var creado = await repo.CrearAsync(egreso);
            return Results.Json(new { ok = true, data = creado }, statusCode: 201);
        })
        .WithName("RegistrarEgreso")
        .WithSummary("Registra un nuevo egreso");

        group.MapDelete("/{id:int}", async (int id, IEgresoRepository repo) =>
        {
            var ok = await repo.EliminarAsync(id);
            return ok
                ? Results.Json(new { ok = true })
                : Results.Json(new { error = "Egreso no encontrado." }, statusCode: 404);
        })
        .WithName("EliminarEgreso")
        .WithSummary("Elimina un egreso");
    }
}
