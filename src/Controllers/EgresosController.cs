using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Models.Enums;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Controllers;

public class EgresosController(IEgresoRepository repo) : BaseController
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

    [HttpGet("categorias")]
    public async Task<IActionResult> Categorias(TipoCategoria? tipo = TipoCategoria.Egreso, bool incluirInactivas = false) =>
        ApiOk((await repo.ObtenerCategoriasAsync(tipo, incluirInactivas))
            .Select(c => new { key = c.Id, label = c.Nombre, c.Tipo, c.Activo }));

    [HttpPost("categorias")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> CrearCategoria([FromBody] CrearCategoriaRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Nombre))
            throw new ArgumentException("El nombre de la categoría es obligatorio.");

        var categoria = new Categoria { Nombre = req.Nombre.Trim(), Tipo = req.Tipo };
        return ApiCreated(await repo.CrearCategoriaAsync(categoria));
    }

    [HttpPut("categorias/{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> ActualizarCategoria(int id, [FromBody] ActualizarCategoriaRequest req)
    {
        var categoria = await repo.ObtenerCategoriaPorIdAsync(id)
            ?? throw new KeyNotFoundException("Categoría no encontrada.");

        if (string.IsNullOrWhiteSpace(req.Nombre))
            throw new ArgumentException("El nombre de la categoría es obligatorio.");

        categoria.Nombre = req.Nombre.Trim();
        categoria.Tipo = req.Tipo;
        categoria.Activo = req.Activo;
        await repo.ActualizarCategoriaAsync(categoria);
        return ApiOk(categoria);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var egreso = await repo.ObtenerPorIdAsync(id)
            ?? throw new KeyNotFoundException("Egreso no encontrado.");
        return ApiOk(egreso);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEgresoRequest req)
    {
        if (!await repo.ExisteCategoriaActivaAsync(req.CategoriaId, TipoCategoria.Egreso))
            throw new ArgumentException("La categoría no existe, está inactiva o no aplica para egresos.");

        var egreso = new Egreso
        {
            Fecha = req.Fecha,
            CategoriaId = req.CategoriaId,
            Descripcion = req.Descripcion,
            Monto = req.Monto,
            Proveedor = req.Proveedor,
            Comprobante = req.Comprobante,
            Observaciones = req.Observaciones
        };
        return ApiCreated(await repo.CrearAsync(egreso));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!await repo.EliminarAsync(id))
            throw new KeyNotFoundException("Egreso no encontrado.");
        return ApiOk();
    }
}
