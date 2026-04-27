using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Models.Enums;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IEgresoRepository
{
    Task<IEnumerable<Egreso>> ObtenerTodosAsync();
    Task<IEnumerable<Egreso>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta);
    Task<PagedResult<Egreso>> ObtenerPaginadoAsync(int page, int pageSize);
    Task<PagedResult<Egreso>> ObtenerPorFechaPaginadoAsync(DateTime desde, DateTime hasta, int page, int pageSize);
    Task<Egreso?> ObtenerPorIdAsync(int id);
    Task<Egreso> CrearAsync(Egreso egreso);
    Task<bool> EliminarAsync(int id);
    Task<IEnumerable<Categoria>> ObtenerCategoriasAsync(TipoCategoria? tipo = null, bool incluirInactivas = false);
    Task<Categoria?> ObtenerCategoriaPorIdAsync(int id);
    Task<bool> ExisteCategoriaActivaAsync(int id, TipoCategoria tipo);
    Task<Categoria> CrearCategoriaAsync(Categoria categoria);
    Task ActualizarCategoriaAsync(Categoria categoria);
}
