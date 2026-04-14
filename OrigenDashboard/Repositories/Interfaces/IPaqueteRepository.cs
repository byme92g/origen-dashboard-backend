using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IPaqueteRepository
{
    Task<IEnumerable<Paquete>> ObtenerTodosAsync();
    Task<PagedResult<Paquete>> ObtenerPaginadoAsync(int page, int pageSize);
    Task<Paquete?> ObtenerPorIdAsync(int id);
    Task<Paquete> CrearAsync(Paquete paquete, IEnumerable<int> servicioIds, IEnumerable<int> productoIds);
    Task<bool> ActualizarAsync(Paquete paquete, IEnumerable<int> servicioIds, IEnumerable<int> productoIds);
    Task<bool> EliminarAsync(int id);
}
