using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Endpoints;

public static class PaquetesEndpoints
{
    public static void MapPaquetesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/paquetes")
            .WithTags("Paquetes")
            .RequireAuthorization();

        group.MapGet("/", async (IPaqueteRepository repo, int? page, int? pageSize) =>
        {
            if (page.HasValue && pageSize.HasValue)
            {
                var ps = Math.Clamp(pageSize.Value, 1, 100);
                return Results.Json(new { ok = true, data = await repo.ObtenerPaginadoAsync(page.Value, ps) });
            }
            return Results.Json(new { ok = true, data = await repo.ObtenerTodosAsync() });
        })
        .WithName("ListarPaquetes")
        .WithSummary("Lista todos los paquetes con sus servicios");

        group.MapGet("/{id:int}", async (int id, IPaqueteRepository repo) =>
        {
            var p = await repo.ObtenerPorIdAsync(id);
            return p is null
                ? Results.Json(new { error = "Paquete no encontrado." }, statusCode: 404)
                : Results.Json(new { ok = true, data = p });
        })
        .WithName("ObtenerPaquete")
        .WithSummary("Obtiene un paquete por ID");

        group.MapPost("/", async (CrearPaqueteRequest req, IPaqueteRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(req.Nombre))
                return Results.Json(new { error = "El nombre es requerido." }, statusCode: 400);

            var paquete = new Paquete
            {
                Nombre = req.Nombre,
                Descripcion = req.Descripcion,
                Precio = req.Precio,
                Descuento = req.Descuento,
                ComisionPct = req.ComisionPct
            };

            var creado = await repo.CrearAsync(paquete, req.ServicioIds, req.ProductoIds);
            return Results.Json(new { ok = true, data = creado }, statusCode: 201);
        })
        .WithName("CrearPaquete")
        .WithSummary("Crea un nuevo paquete")
        .RequireAuthorization(policy => policy.RequireRole("admin"));

        group.MapPut("/{id:int}", async (int id, ActualizarPaqueteRequest req, IPaqueteRepository repo) =>
        {
            var paquete = await repo.ObtenerPorIdAsync(id);
            if (paquete is null)
                return Results.Json(new { error = "Paquete no encontrado." }, statusCode: 404);

            paquete.Nombre = req.Nombre;
            paquete.Descripcion = req.Descripcion;
            paquete.Precio = req.Precio;
            paquete.Descuento = req.Descuento;
            paquete.ComisionPct = req.ComisionPct;
            paquete.Activo = req.Activo;

            await repo.ActualizarAsync(paquete, req.ServicioIds, req.ProductoIds);
            return Results.Json(new { ok = true, data = paquete });
        })
        .WithName("ActualizarPaquete")
        .WithSummary("Actualiza un paquete y sus servicios")
        .RequireAuthorization(policy => policy.RequireRole("admin"));

        group.MapDelete("/{id:int}", async (int id, IPaqueteRepository repo) =>
        {
            var ok = await repo.EliminarAsync(id);
            return ok
                ? Results.Json(new { ok = true })
                : Results.Json(new { error = "Paquete no encontrado." }, statusCode: 404);
        })
        .WithName("EliminarPaquete")
        .WithSummary("Elimina un paquete")
        .RequireAuthorization(policy => policy.RequireRole("admin"));
    }
}
