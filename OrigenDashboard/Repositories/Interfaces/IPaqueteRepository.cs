using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IPaqueteRepository
{
    Task<IEnumerable<Paquete>> ObtenerTodosAsync();
    Task<Paquete?> ObtenerPorIdAsync(int id);
    Task<Paquete> CrearAsync(Paquete paquete, IEnumerable<int> servicioIds);
    Task<bool> ActualizarAsync(Paquete paquete, IEnumerable<int> servicioIds);
    Task<bool> EliminarAsync(int id);
}
