using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Endpoints;

public static class EmpleadosEndpoints
{
    public static void MapEmpleadosEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/empleados")
            .WithTags("Empleados")
            .RequireAuthorization();

        group.MapGet("/", async (IEmpleadoRepository repo) =>
        {
            var lista = await repo.ObtenerTodosAsync();
            return Results.Json(new { ok = true, data = lista });
        })
        .WithName("ListarEmpleados")
        .WithSummary("Lista todos los empleados");

        group.MapGet("/{id:int}", async (int id, IEmpleadoRepository repo) =>
        {
            var empleado = await repo.ObtenerPorIdAsync(id);
            return empleado is null
                ? Results.Json(new { error = "Empleado no encontrado." }, statusCode: 404)
                : Results.Json(new { ok = true, data = empleado });
        })
        .WithName("ObtenerEmpleado")
        .WithSummary("Obtiene un empleado por ID");

        group.MapPost("/", async (CrearEmpleadoRequest req, IEmpleadoRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(req.Nombre))
                return Results.Json(new { error = "El nombre es requerido." }, statusCode: 400);

            var empleado = new Empleado
            {
                Nombre = req.Nombre,
                Cargo = req.Cargo,
                ComisionPct = req.ComisionPct,
                UsuarioLogin = req.UsuarioLogin
            };

            var creado = await repo.CrearAsync(empleado);
            return Results.Json(new { ok = true, data = creado }, statusCode: 201);
        })
        .WithName("CrearEmpleado")
        .WithSummary("Crea un nuevo empleado");

        group.MapPut("/{id:int}", async (int id, ActualizarEmpleadoRequest req, IEmpleadoRepository repo) =>
        {
            var empleado = await repo.ObtenerPorIdAsync(id);
            if (empleado is null)
                return Results.Json(new { error = "Empleado no encontrado." }, statusCode: 404);

            empleado.Nombre = req.Nombre;
            empleado.Cargo = req.Cargo;
            empleado.ComisionPct = req.ComisionPct;
            empleado.UsuarioLogin = req.UsuarioLogin;
            empleado.Activo = req.Activo;

            await repo.ActualizarAsync(empleado);
            return Results.Json(new { ok = true, data = empleado });
        })
        .WithName("ActualizarEmpleado")
        .WithSummary("Actualiza un empleado");

        group.MapDelete("/{id:int}", async (int id, IEmpleadoRepository repo) =>
        {
            var ok = await repo.EliminarAsync(id);
            return ok
                ? Results.Json(new { ok = true })
                : Results.Json(new { error = "Empleado no encontrado." }, statusCode: 404);
        })
        .WithName("EliminarEmpleado")
        .WithSummary("Elimina un empleado");
    }
}
