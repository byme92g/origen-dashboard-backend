using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Endpoints;

public static class ServiciosEndpoints
{
    public static void MapServiciosEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/servicios")
            .WithTags("Servicios")
            .RequireAuthorization();

        group.MapGet("/", async (IServicioRepository repo) =>
            Results.Json(new { ok = true, data = await repo.ObtenerTodosAsync() }))
        .WithName("ListarServicios")
        .WithSummary("Lista todos los servicios");

        group.MapGet("/{id:int}", async (int id, IServicioRepository repo) =>
        {
            var s = await repo.ObtenerPorIdAsync(id);
            return s is null
                ? Results.Json(new { error = "Servicio no encontrado." }, statusCode: 404)
                : Results.Json(new { ok = true, data = s });
        })
        .WithName("ObtenerServicio")
        .WithSummary("Obtiene un servicio por ID");

        group.MapPost("/", async (CrearServicioRequest req, IServicioRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(req.Nombre))
                return Results.Json(new { error = "El nombre es requerido." }, statusCode: 400);

            var servicio = new Servicio
            {
                Nombre = req.Nombre,
                Categoria = req.Categoria,
                Precio = req.Precio,
                DuracionMin = req.DuracionMin,
                ComisionPct = req.ComisionPct
            };

            var creado = await repo.CrearAsync(servicio);
            return Results.Json(new { ok = true, data = creado }, statusCode: 201);
        })
        .WithName("CrearServicio")
        .WithSummary("Crea un nuevo servicio");

        group.MapPut("/{id:int}", async (int id, ActualizarServicioRequest req, IServicioRepository repo) =>
        {
            var servicio = await repo.ObtenerPorIdAsync(id);
            if (servicio is null)
                return Results.Json(new { error = "Servicio no encontrado." }, statusCode: 404);

            servicio.Nombre = req.Nombre;
            servicio.Categoria = req.Categoria;
            servicio.Precio = req.Precio;
            servicio.DuracionMin = req.DuracionMin;
            servicio.ComisionPct = req.ComisionPct;
            servicio.Activo = req.Activo;

            await repo.ActualizarAsync(servicio);
            return Results.Json(new { ok = true, data = servicio });
        })
        .WithName("ActualizarServicio")
        .WithSummary("Actualiza un servicio");

        group.MapDelete("/{id:int}", async (int id, IServicioRepository repo) =>
        {
            var ok = await repo.EliminarAsync(id);
            return ok
                ? Results.Json(new { ok = true })
                : Results.Json(new { error = "Servicio no encontrado." }, statusCode: 404);
        })
        .WithName("EliminarServicio")
        .WithSummary("Elimina un servicio");
    }
}
