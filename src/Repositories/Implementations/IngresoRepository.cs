using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class IngresoRepository(AppDbContext db) : IIngresoRepository
{
    private IQueryable<Ingreso> ConIncludes() =>
        db.Ingresos
            .Include(i => i.Cliente)
            .Include(i => i.Empleado)
            .Include(i => i.Servicio)
            .Include(i => i.Producto)
            .Include(i => i.Paquete);

    public async Task<IEnumerable<Ingreso>> ObtenerTodosAsync() =>
        await ConIncludes().OrderByDescending(i => i.Fecha).ToListAsync();

    public async Task<IEnumerable<Ingreso>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta) =>
        await ConIncludes()
            .Where(i => i.Fecha >= desde && i.Fecha <= hasta)
            .OrderByDescending(i => i.Fecha)
            .ToListAsync();

    public async Task<PagedResult<Ingreso>> ObtenerPaginadoAsync(int page, int pageSize)
    {
        var q = ConIncludes().OrderByDescending(i => i.Fecha);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Ingreso>(items, total, page, pageSize);
    }

    public async Task<PagedResult<Ingreso>> ObtenerPorFechaPaginadoAsync(DateTime desde, DateTime hasta, int page, int pageSize)
    {
        var q = ConIncludes()
            .Where(i => i.Fecha >= desde && i.Fecha <= hasta)
            .OrderByDescending(i => i.Fecha);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Ingreso>(items, total, page, pageSize);
    }

    public async Task<Ingreso?> ObtenerPorIdAsync(int id) =>
        await ConIncludes().FirstOrDefaultAsync(i => i.Id == id);

    public async Task<Ingreso> CrearAsync(Ingreso ingreso)
    {
        db.Ingresos.Add(ingreso);
        await db.SaveChangesAsync();
        return ingreso;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var ingreso = await db.Ingresos.FindAsync(id);
        if (ingreso is null) return false;
        db.Ingresos.Remove(ingreso);
        return await db.SaveChangesAsync() > 0;
    }
}
