using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class PaqueteRepository(AppDbContext db) : IPaqueteRepository
{
    public async Task<IEnumerable<Paquete>> ObtenerTodosAsync() =>
        await db.Paquetes
            .Include(p => p.Servicios).ThenInclude(ps => ps.Servicio)
            .Include(p => p.Productos).ThenInclude(pp => pp.Producto)
            .OrderBy(p => p.Nombre)
            .ToListAsync();

    public async Task<PagedResult<Paquete>> ObtenerPaginadoAsync(int page, int pageSize)
    {
        var q = db.Paquetes
            .Include(p => p.Servicios).ThenInclude(ps => ps.Servicio)
            .Include(p => p.Productos).ThenInclude(pp => pp.Producto)
            .OrderBy(p => p.Nombre);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Paquete>(items, total, page, pageSize);
    }

    public async Task<Paquete?> ObtenerPorIdAsync(int id) =>
        await db.Paquetes
            .Include(p => p.Servicios).ThenInclude(ps => ps.Servicio)
            .Include(p => p.Productos).ThenInclude(pp => pp.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Paquete> CrearAsync(Paquete paquete, IEnumerable<int> servicioIds, IEnumerable<int> productoIds)
    {
        db.Paquetes.Add(paquete);
        await db.SaveChangesAsync();

        foreach (var sid in servicioIds)
            db.PaqueteServicios.Add(new PaqueteServicio { PaqueteId = paquete.Id, ServicioId = sid });

        foreach (var pid in productoIds)
            db.PaqueteProductos.Add(new PaqueteProducto { PaqueteId = paquete.Id, ProductoId = pid });

        await db.SaveChangesAsync();
        return paquete;
    }

    public async Task<bool> ActualizarAsync(Paquete paquete, IEnumerable<int> servicioIds, IEnumerable<int> productoIds)
    {
        // Mark only the scalar properties as modified — avoid cascading Update() into navigation properties
        db.Entry(paquete).State = EntityState.Modified;

        // Delete existing junction rows by marking each tracked entity as Deleted
        foreach (var s in paquete.Servicios.ToList())
            db.Entry(s).State = EntityState.Deleted;

        foreach (var p in paquete.Productos.ToList())
            db.Entry(p).State = EntityState.Deleted;

        foreach (var sid in servicioIds.Distinct())
            db.PaqueteServicios.Add(new PaqueteServicio { PaqueteId = paquete.Id, ServicioId = sid });

        foreach (var pid in productoIds.Distinct())
            db.PaqueteProductos.Add(new PaqueteProducto { PaqueteId = paquete.Id, ProductoId = pid });

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
