using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class ServicioRepository(AppDbContext db) : IServicioRepository
{
    public async Task<IEnumerable<Servicio>> ObtenerTodosAsync() =>
        await db.Servicios.OrderBy(s => s.Categoria).ThenBy(s => s.Nombre).ToListAsync();

    public async Task<Servicio?> ObtenerPorIdAsync(int id) =>
        await db.Servicios.FindAsync(id);

    public async Task<Servicio> CrearAsync(Servicio servicio)
    {
        db.Servicios.Add(servicio);
        await db.SaveChangesAsync();
        return servicio;
    }

    public async Task<bool> ActualizarAsync(Servicio servicio)
    {
        db.Servicios.Update(servicio);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var servicio = await db.Servicios.FindAsync(id);
        if (servicio is null) return false;
        db.Servicios.Remove(servicio);
        return await db.SaveChangesAsync() > 0;
    }
}
