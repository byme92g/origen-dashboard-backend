using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Models.Enums;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class IngresosController(IIngresoRepository repo, IProductoRepository productoRepo) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Listar(DateTime? desde, DateTime? hasta, int? page, int? pageSize)
    {
        if (page.HasValue && pageSize.HasValue)
        {
            var ps = Math.Clamp(pageSize.Value, 1, 100);
            var result = desde.HasValue && hasta.HasValue
                ? await repo.ObtenerPorFechaPaginadoAsync(desde.Value, hasta.Value, page.Value, ps)
                : await repo.ObtenerPaginadoAsync(page.Value, ps);
            return ApiOk(result);
        }
        var lista = desde.HasValue && hasta.HasValue
            ? await repo.ObtenerPorFechaAsync(desde.Value, hasta.Value)
            : await repo.ObtenerTodosAsync();
        return ApiOk(lista);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var ingreso = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Ingreso no encontrado.");
        return ApiOk(ingreso);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearIngresoRequest req)
    {
        var cantidad = req.Cantidad < 1 ? 1 : req.Cantidad;

        if (req.Tipo == TipoIngreso.Producto && req.ProductoId.HasValue)
        {
            var producto = await productoRepo.ObtenerPorIdAsync(req.ProductoId.Value)
                ?? throw new KeyNotFoundException("Producto no encontrado.");

            if (producto.Stock < cantidad)
                throw new ArgumentException($"Stock insuficiente. Disponible: {producto.Stock}.");

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

        return ApiCreated(await repo.CrearAsync(ingreso));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!await repo.EliminarAsync(id))
            throw new KeyNotFoundException("Ingreso no encontrado.");
        return ApiOk();
    }
}
