using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Endpoints;

public static class ClientesEndpoints
{
    public static void MapClientesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/clientes")
            .WithTags("Clientes")
            .RequireAuthorization();

        group.MapGet("/", async (IClienteRepository repo, int? page, int? pageSize) =>
        {
            if (page.HasValue && pageSize.HasValue)
            {
                var ps = Math.Clamp(pageSize.Value, 1, 100);
                var result = await repo.ObtenerPaginadoAsync(page.Value, ps);
                return Results.Json(new { ok = true, data = result });
            }
            var lista = await repo.ObtenerTodosAsync();
            return Results.Json(new { ok = true, data = lista });
        })
        .WithName("ListarClientes")
        .WithSummary("Lista todos los clientes");

        group.MapGet("/{id:int}", async (int id, IClienteRepository repo) =>
        {
            var cliente = await repo.ObtenerPorIdAsync(id);
            return cliente is null
                ? Results.Json(new { error = "Cliente no encontrado." }, statusCode: 404)
                : Results.Json(new { ok = true, data = cliente });
        })
        .WithName("ObtenerCliente")
        .WithSummary("Obtiene un cliente por ID");

        group.MapPost("/", async (CrearClienteRequest req, IClienteRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(req.Nombre))
                return Results.Json(new { error = "El nombre es requerido." }, statusCode: 400);

            var cliente = new Cliente
            {
                Nombre = req.Nombre,
                Telefono = req.Telefono,
                Email = req.Email,
                Observaciones = req.Observaciones
            };

            var creado = await repo.CrearAsync(cliente);
            return Results.Json(new { ok = true, data = creado }, statusCode: 201);
        })
        .WithName("CrearCliente")
        .WithSummary("Crea un nuevo cliente");

        group.MapPut("/{id:int}", async (int id, ActualizarClienteRequest req, IClienteRepository repo) =>
        {
            var cliente = await repo.ObtenerPorIdAsync(id);
            if (cliente is null)
                return Results.Json(new { error = "Cliente no encontrado." }, statusCode: 404);

            cliente.Nombre = req.Nombre;
            cliente.Telefono = req.Telefono;
            cliente.Email = req.Email;
            cliente.Observaciones = req.Observaciones;

            await repo.ActualizarAsync(cliente);
            return Results.Json(new { ok = true, data = cliente });
        })
        .WithName("ActualizarCliente")
        .WithSummary("Actualiza un cliente");

        group.MapDelete("/{id:int}", async (int id, IClienteRepository repo) =>
        {
            var ok = await repo.EliminarAsync(id);
            return ok
                ? Results.Json(new { ok = true })
                : Results.Json(new { error = "Cliente no encontrado." }, statusCode: 404);
        })
        .WithName("EliminarCliente")
        .WithSummary("Elimina un cliente")
        .RequireAuthorization(policy => policy.RequireRole("admin"));
    }
}
