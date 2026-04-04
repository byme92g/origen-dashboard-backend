using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class UsuarioRepository(AppDbContext db) : IUsuarioRepository
{
    public async Task<Usuario?> ObtenerPorUsuarioAsync(string nombreUsuario) =>
        await db.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

    public async Task<Usuario?> ObtenerPorIdAsync(int id) =>
        await db.Usuarios.FindAsync(id);

    public async Task<IEnumerable<Usuario>> ObtenerTodosAsync() =>
        await db.Usuarios.OrderBy(u => u.NombreCompleto).ToListAsync();

    public async Task<Usuario> CrearAsync(Usuario usuario)
    {
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> ActualizarAsync(Usuario usuario)
    {
        db.Usuarios.Update(usuario);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var usuario = await db.Usuarios.FindAsync(id);
        if (usuario is null) return false;
        db.Usuarios.Remove(usuario);
        return await db.SaveChangesAsync() > 0;
    }
}
