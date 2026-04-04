using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IServicioRepository
{
    Task<IEnumerable<Servicio>> ObtenerTodosAsync();
    Task<Servicio?> ObtenerPorIdAsync(int id);
    Task<Servicio> CrearAsync(Servicio servicio);
    Task<bool> ActualizarAsync(Servicio servicio);
    Task<bool> EliminarAsync(int id);
}
