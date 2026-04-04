using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class PaqueteRepository(AppDbContext db) : IPaqueteRepository
{
    public async Task<IEnumerable<Paquete>> ObtenerTodosAsync() =>
        await db.Paquetes
            .Include(p => p.Servicios).ThenInclude(ps => ps.Servicio)
            .OrderBy(p => p.Nombre)
            .ToListAsync();

    public async Task<Paquete?> ObtenerPorIdAsync(int id) =>
        await db.Paquetes
            .Include(p => p.Servicios).ThenInclude(ps => ps.Servicio)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Paquete> CrearAsync(Paquete paquete, IEnumerable<int> servicioIds)
    {
        db.Paquetes.Add(paquete);
        await db.SaveChangesAsync();

        foreach (var sid in servicioIds)
            db.PaqueteServicios.Add(new PaqueteServicio { PaqueteId = paquete.Id, ServicioId = sid });

        await db.SaveChangesAsync();
        return paquete;
    }

    public async Task<bool> ActualizarAsync(Paquete paquete, IEnumerable<int> servicioIds)
    {
        db.Paquetes.Update(paquete);

        var existentes = db.PaqueteServicios.Where(ps => ps.PaqueteId == paquete.Id);
        db.PaqueteServicios.RemoveRange(existentes);

        foreach (var sid in servicioIds)
            db.PaqueteServicios.Add(new PaqueteServicio { PaqueteId = paquete.Id, ServicioId = sid });

        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var paquete = await db.Paquetes.FindAsync(id);
        if (paquete is null) return false;
        db.Paquetes.Remove(paquete);
        return await db.SaveChangesAsync() > 0;
    }
}
