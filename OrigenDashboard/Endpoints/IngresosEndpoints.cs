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

        group.MapGet("/", async (IIngresoRepository repo, DateTime? desde, DateTime? hasta, int? page, int? pageSize) =>
        {
            if (page.HasValue && pageSize.HasValue)
            {
                var ps = Math.Clamp(pageSize.Value, 1, 100);
                var result = desde.HasValue && hasta.HasValue
                    ? await repo.ObtenerPorFechaPaginadoAsync(desde.Value, hasta.Value, page.Value, ps)
                    : await repo.ObtenerPaginadoAsync(page.Value, ps);
                return Results.Json(new { ok = true, data = result });
            }
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

        group.MapPost("/", async (CrearIngresoRequest req, IIngresoRepository repo, IProductoRepository productoRepo) =>
        {
            if (req.Monto <= 0)
                return Results.Json(new { error = "El monto debe ser mayor a 0." }, statusCode: 400);

            if (string.IsNullOrWhiteSpace(req.MetodoPago))
                return Results.Json(new { error = "El método de pago es requerido." }, statusCode: 400);

            var cantidad = req.Cantidad < 1 ? 1 : req.Cantidad;

            // Validar y descontar stock si es venta de producto
            if (req.Tipo == "producto" && req.ProductoId.HasValue)
            {
                var producto = await productoRepo.ObtenerPorIdAsync(req.ProductoId.Value);
                if (producto is null)
                    return Results.Json(new { error = "Producto no encontrado." }, statusCode: 404);

                if (producto.Stock < cantidad)
                    return Results.Json(new { error = $"Stock insuficiente. Disponible: {producto.Stock}." }, statusCode: 400);

                producto.Stock -= cantidad;
                await productoRepo.ActualizarAsync(producto);
            }

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
                Cantidad = cantidad,
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
        .WithSummary("Elimina un ingreso")
        .RequireAuthorization(policy => policy.RequireRole("admin"));
    }
}
