using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class EgresoRepository(AppDbContext db) : IEgresoRepository
{
    public async Task<IEnumerable<Egreso>> ObtenerTodosAsync() =>
        await db.Egresos.OrderByDescending(e => e.Fecha).ToListAsync();

    public async Task<IEnumerable<Egreso>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta) =>
        await db.Egresos
            .Where(e => e.Fecha >= desde && e.Fecha <= hasta)
            .OrderByDescending(e => e.Fecha)
            .ToListAsync();

    public async Task<Egreso?> ObtenerPorIdAsync(int id) =>
        await db.Egresos.FindAsync(id);

    public async Task<Egreso> CrearAsync(Egreso egreso)
    {
        db.Egresos.Add(egreso);
        await db.SaveChangesAsync();
        return egreso;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var egreso = await db.Egresos.FindAsync(id);
        if (egreso is null) return false;
        db.Egresos.Remove(egreso);
        return await db.SaveChangesAsync() > 0;
    }
}
