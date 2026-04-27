using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class ClientesController(IClienteRepository repo) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Listar(int? page, int? pageSize)
    {
        if (page.HasValue && pageSize.HasValue)
        {
            var ps = Math.Clamp(pageSize.Value, 1, 100);
            return ApiOk(await repo.ObtenerPaginadoAsync(page.Value, ps));
        }
        return ApiOk(await repo.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var cliente = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");
        return ApiOk(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearClienteRequest req)
    {
        var cliente = new Cliente
        {
            Nombre = req.Nombre,
            Telefono = req.Telefono,
            Email = req.Email,
            Observaciones = req.Observaciones
        };
        return ApiCreated(await repo.CrearAsync(cliente));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarClienteRequest req)
    {
        var cliente = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        cliente.Nombre = req.Nombre;
        cliente.Telefono = req.Telefono;
        cliente.Email = req.Email;
        cliente.Observaciones = req.Observaciones;

        await repo.ActualizarAsync(cliente);
        return ApiOk(cliente);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!await repo.EliminarAsync(id))
            throw new KeyNotFoundException("Cliente no encontrado.");
        return ApiOk();
    }
}
