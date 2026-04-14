using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IEmpleadoRepository
{
    Task<IEnumerable<Empleado>> ObtenerTodosAsync();
    Task<PagedResult<Empleado>> ObtenerPaginadoAsync(int page, int pageSize);
    Task<Empleado?> ObtenerPorIdAsync(int id);
    Task<Empleado> CrearAsync(Empleado empleado);
    Task<bool> ActualizarAsync(Empleado empleado);
    Task<bool> EliminarAsync(int id);
}
