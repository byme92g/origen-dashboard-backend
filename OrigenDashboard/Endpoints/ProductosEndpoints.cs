using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Endpoints;

public static class ProductosEndpoints
{
    public static void MapProductosEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/productos")
            .WithTags("Productos")
            .RequireAuthorization();

        group.MapGet("/", async (IProductoRepository repo, int? page, int? pageSize) =>
        {
            if (page.HasValue && pageSize.HasValue)
            {
                var ps = Math.Clamp(pageSize.Value, 1, 100);
                return Results.Json(new { ok = true, data = await repo.ObtenerPaginadoAsync(page.Value, ps) });
            }
            return Results.Json(new { ok = true, data = await repo.ObtenerTodosAsync() });
        })
        .WithName("ListarProductos")
        .WithSummary("Lista todos los productos");

        group.MapGet("/{id:int}", async (int id, IProductoRepository repo) =>
        {
            var p = await repo.ObtenerPorIdAsync(id);
            return p is null
                ? Results.Json(new { error = "Producto no encontrado." }, statusCode: 404)
                : Results.Json(new { ok = true, data = p });
        })
        .WithName("ObtenerProducto")
        .WithSummary("Obtiene un producto por ID");

        group.MapPost("/", async (CrearProductoRequest req, IProductoRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(req.Nombre))
                return Results.Json(new { error = "El nombre es requerido." }, statusCode: 400);

            var producto = new Producto
            {
                Nombre = req.Nombre,
                Categoria = req.Categoria,
                PrecioVenta = req.PrecioVenta,
                Stock = req.StockInicial
            };

            var creado = await repo.CrearAsync(producto);
            return Results.Json(new { ok = true, data = creado }, statusCode: 201);
        })
        .WithName("CrearProducto")
        .WithSummary("Crea un nuevo producto")
        .RequireAuthorization(policy => policy.RequireRole("admin"));

        group.MapPut("/{id:int}", async (int id, ActualizarProductoRequest req, IProductoRepository repo) =>
        {
            var producto = await repo.ObtenerPorIdAsync(id);
            if (producto is null)
                return Results.Json(new { error = "Producto no encontrado." }, statusCode: 404);

            producto.Nombre = req.Nombre;
            producto.Categoria = req.Categoria;
            producto.PrecioVenta = req.PrecioVenta;
            producto.Stock = req.Stock;
            producto.Activo = req.Activo;

            await repo.ActualizarAsync(producto);
            return Results.Json(new { ok = true, data = producto });
        })
        .WithName("ActualizarProducto")
        .WithSummary("Actualiza un producto")
        .RequireAuthorization(policy => policy.RequireRole("admin"));

        group.MapDelete("/{id:int}", async (int id, IProductoRepository repo) =>
        {
            var ok = await repo.EliminarAsync(id);
            return ok
                ? Results.Json(new { ok = true })
                : Results.Json(new { error = "Producto no encontrado." }, statusCode: 404);
        })
        .WithName("EliminarProducto")
        .WithSummary("Elimina un producto")
        .RequireAuthorization(policy => policy.RequireRole("admin"));
    }
}
