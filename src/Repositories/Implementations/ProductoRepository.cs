using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class ProductoRepository(AppDbContext db) : IProductoRepository
{
    public async Task<IEnumerable<Producto>> ObtenerTodosAsync() =>
        await db.Productos.OrderBy(p => p.Nombre).ToListAsync();

    public async Task<PagedResult<Producto>> ObtenerPaginadoAsync(int page, int pageSize)
    {
        var q = db.Productos.OrderBy(p => p.Nombre);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Producto>(items, total, page, pageSize);
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id) =>
        await db.Productos.FindAsync(id);

    public async Task<Producto> CrearAsync(Producto producto)
    {
        db.Productos.Add(producto);
        await db.SaveChangesAsync();
        return producto;
    }

    public async Task<bool> ActualizarAsync(Producto producto)
    {
        db.Productos.Update(producto);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var producto = await db.Productos.FindAsync(id);
        if (producto is null) return false;
        db.Productos.Remove(producto);
        return await db.SaveChangesAsync() > 0;
    }
}
