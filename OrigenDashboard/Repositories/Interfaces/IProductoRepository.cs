using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IProductoRepository
{
    Task<IEnumerable<Producto>> ObtenerTodosAsync();
    Task<PagedResult<Producto>> ObtenerPaginadoAsync(int page, int pageSize);
    Task<Producto?> ObtenerPorIdAsync(int id);
    Task<Producto> CrearAsync(Producto producto);
    Task<bool> ActualizarAsync(Producto producto);
    Task<bool> EliminarAsync(int id);
}
