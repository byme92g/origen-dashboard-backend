using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class ProductosController(IProductoRepository repo) : BaseController
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
        var producto = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Producto no encontrado.");
        return ApiOk(producto);
    }

    [HttpPost]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Crear([FromBody] CrearProductoRequest req)
    {
        var producto = new Producto
        {
            Nombre = req.Nombre,
            Categoria = req.Categoria,
            PrecioVenta = req.PrecioVenta,
            Stock = req.StockInicial
        };
        return ApiCreated(await repo.CrearAsync(producto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarProductoRequest req)
    {
        var producto = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Producto no encontrado.");

        producto.Nombre = req.Nombre;
        producto.Categoria = req.Categoria;
        producto.PrecioVenta = req.PrecioVenta;
        producto.Stock = req.Stock;
        producto.Activo = req.Activo;

        await repo.ActualizarAsync(producto);
        return ApiOk(producto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!await repo.EliminarAsync(id))
            throw new KeyNotFoundException("Producto no encontrado.");
        return ApiOk();
    }
}
