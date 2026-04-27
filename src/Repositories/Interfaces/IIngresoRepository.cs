using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IIngresoRepository
{
    Task<IEnumerable<Ingreso>> ObtenerTodosAsync();
    Task<IEnumerable<Ingreso>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta);
    Task<PagedResult<Ingreso>> ObtenerPaginadoAsync(int page, int pageSize);
    Task<PagedResult<Ingreso>> ObtenerPorFechaPaginadoAsync(DateTime desde, DateTime hasta, int page, int pageSize);
    Task<Ingreso?> ObtenerPorIdAsync(int id);
    Task<Ingreso> CrearAsync(Ingreso ingreso);
    Task<bool> EliminarAsync(int id);
}
