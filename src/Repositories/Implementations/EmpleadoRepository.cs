using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class EmpleadoRepository(AppDbContext db) : IEmpleadoRepository
{
    public async Task<IEnumerable<Empleado>> ObtenerTodosAsync() =>
        await db.Empleados.OrderBy(e => e.Nombre).ToListAsync();

    public async Task<PagedResult<Empleado>> ObtenerPaginadoAsync(int page, int pageSize)
    {
        var q = db.Empleados.OrderBy(e => e.Nombre);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Empleado>(items, total, page, pageSize);
    }

    public async Task<Empleado?> ObtenerPorIdAsync(int id) =>
        await db.Empleados.FindAsync(id);

    public async Task<Empleado> CrearAsync(Empleado empleado)
    {
        db.Empleados.Add(empleado);
        await db.SaveChangesAsync();
        return empleado;
    }

    public async Task<bool> ActualizarAsync(Empleado empleado)
    {
        db.Empleados.Update(empleado);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var empleado = await db.Empleados.FindAsync(id);
        if (empleado is null) return false;
        db.Empleados.Remove(empleado);
        return await db.SaveChangesAsync() > 0;
    }
}
