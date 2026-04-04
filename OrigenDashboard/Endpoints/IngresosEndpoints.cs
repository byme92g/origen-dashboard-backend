using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Endpoints;

public static class IngresosEndpoints
{
    public static void MapIngresosEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/ingresos")
            .WithTags("Ingresos")
            .RequireAuthorization();

        group.MapGet("/", async (IIngresoRepository repo, DateTime? desde, DateTime? hasta) =>
        {
            var lista = desde.HasValue && hasta.HasValue
                ? await repo.ObtenerPorFechaAsync(desde.Value, hasta.Value)
                : await repo.ObtenerTodosAsync();

            return Results.Json(new { ok = true, data = lista });
        })
        .WithName("ListarIngresos")
        .WithSummary("Lista ingresos, opcionalmente filtrados por rango de fecha");

        group.MapGet("/{id:int}", async (int id, IIngresoRepository repo) =>
        {
            var ingreso = await repo.ObtenerPorIdAsync(id);
            return ingreso is null
                ? Results.Json(new { error = "Ingreso no encontrado." }, statusCode: 404)
                : Results.Json(new { ok = true, data = ingreso });
        })
        .WithName("ObtenerIngreso")
        .WithSummary("Obtiene un ingreso por ID");

        group.MapPost("/", async (CrearIngresoRequest req, IIngresoRepository repo) =>
        {
            if (req.Monto <= 0)
                return Results.Json(new { error = "El monto debe ser mayor a 0." }, statusCode: 400);

            if (string.IsNullOrWhiteSpace(req.MetodoPago))
                return Results.Json(new { error = "El método de pago es requerido." }, statusCode: 400);

            var ingreso = new Ingreso
            {
                Fecha = req.Fecha,
                ClienteId = req.ClienteId,
                EmpleadoId = req.EmpleadoId,
                Tipo = req.Tipo,
                ServicioId = req.ServicioId,
                ProductoId = req.ProductoId,
                PaqueteId = req.PaqueteId,
                ConceptoPersonalizado = req.ConceptoPersonalizado,
                Monto = req.Monto,
                Descuento = req.Descuento,
                MetodoPago = req.MetodoPago,
                Referencia = req.Referencia,
                Comision = req.Comision,
                Observaciones = req.Observaciones
            };

            var creado = await repo.CrearAsync(ingreso);
            return Results.Json(new { ok = true, data = creado }, statusCode: 201);
        })
        .WithName("RegistrarIngreso")
        .WithSummary("Registra un nuevo ingreso");

        group.MapDelete("/{id:int}", async (int id, IIngresoRepository repo) =>
        {
            var ok = await repo.EliminarAsync(id);
            return ok
                ? Results.Json(new { ok = true })
                : Results.Json(new { error = "Ingreso no encontrado." }, statusCode: 404);
        })
        .WithName("EliminarIngreso")
        .WithSummary("Elimina un ingreso");
    }
}
