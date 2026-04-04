using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IIngresoRepository
{
    Task<IEnumerable<Ingreso>> ObtenerTodosAsync();
    Task<IEnumerable<Ingreso>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta);
    Task<Ingreso?> ObtenerPorIdAsync(int id);
    Task<Ingreso> CrearAsync(Ingreso ingreso);
    Task<bool> EliminarAsync(int id);
}
