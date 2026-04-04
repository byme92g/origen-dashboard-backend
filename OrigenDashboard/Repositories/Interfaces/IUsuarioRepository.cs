using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorUsuarioAsync(string nombreUsuario);
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Usuario>> ObtenerTodosAsync();
    Task<Usuario> CrearAsync(Usuario usuario);
    Task<bool> ActualizarAsync(Usuario usuario);
    Task<bool> EliminarAsync(int id);
}
