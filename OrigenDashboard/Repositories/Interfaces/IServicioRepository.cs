using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IServicioRepository
{
    Task<IEnumerable<Servicio>> ObtenerTodosAsync();
    Task<PagedResult<Servicio>> ObtenerPaginadoAsync(int page, int pageSize);
    Task<Servicio?> ObtenerPorIdAsync(int id);
    Task<Servicio> CrearAsync(Servicio servicio);
    Task<bool> ActualizarAsync(Servicio servicio);
    Task<bool> EliminarAsync(int id);
}
