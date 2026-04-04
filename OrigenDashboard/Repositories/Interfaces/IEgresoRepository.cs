using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IEgresoRepository
{
    Task<IEnumerable<Egreso>> ObtenerTodosAsync();
    Task<IEnumerable<Egreso>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta);
    Task<Egreso?> ObtenerPorIdAsync(int id);
    Task<Egreso> CrearAsync(Egreso egreso);
    Task<bool> EliminarAsync(int id);
}
