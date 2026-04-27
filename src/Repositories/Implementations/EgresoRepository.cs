using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Models.Enums;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class EgresoRepository(AppDbContext db) : IEgresoRepository
{
    public async Task<IEnumerable<Egreso>> ObtenerTodosAsync() =>
        await db.Egresos.Include(e => e.Categoria).OrderByDescending(e => e.Fecha).ToListAsync();

    public async Task<IEnumerable<Egreso>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta) =>
        await db.Egresos
            .Include(e => e.Categoria)
            .Where(e => e.Fecha >= desde && e.Fecha <= hasta)
            .OrderByDescending(e => e.Fecha)
            .ToListAsync();

    public async Task<PagedResult<Egreso>> ObtenerPaginadoAsync(int page, int pageSize)
    {
        var q = db.Egresos.Include(e => e.Categoria).OrderByDescending(e => e.Fecha);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Egreso>(items, total, page, pageSize);
    }

    public async Task<PagedResult<Egreso>> ObtenerPorFechaPaginadoAsync(DateTime desde, DateTime hasta, int page, int pageSize)
    {
        var q = db.Egresos
            .Include(e => e.Categoria)
            .Where(e => e.Fecha >= desde && e.Fecha <= hasta)
            .OrderByDescending(e => e.Fecha);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Egreso>(items, total, page, pageSize);
    }

    public async Task<Egreso?> ObtenerPorIdAsync(int id) =>
        await db.Egresos.Include(e => e.Categoria).FirstOrDefaultAsync(e => e.Id == id);

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

    public async Task<IEnumerable<Categoria>> ObtenerCategoriasAsync(TipoCategoria? tipo = null, bool incluirInactivas = false)
    {
        var q = db.Categorias.AsQueryable();
        if (tipo.HasValue)
            q = q.Where(c => c.Tipo == tipo.Value || c.Tipo == TipoCategoria.Ambos);

        if (!incluirInactivas)
            q = q.Where(c => c.Activo);

        return await q.OrderBy(c => c.Nombre).ToListAsync();
    }

    public async Task<Categoria?> ObtenerCategoriaPorIdAsync(int id) =>
        await db.Categorias.FindAsync(id);

    public async Task<bool> ExisteCategoriaActivaAsync(int id, TipoCategoria tipo) =>
        await db.Categorias.AnyAsync(c =>
            c.Id == id &&
            c.Activo &&
            (c.Tipo == tipo || c.Tipo == TipoCategoria.Ambos));

    public async Task<Categoria> CrearCategoriaAsync(Categoria categoria)
    {
        db.Categorias.Add(categoria);
        await db.SaveChangesAsync();
        return categoria;
    }

    public async Task ActualizarCategoriaAsync(Categoria categoria)
    {
        db.Categorias.Update(categoria);
        await db.SaveChangesAsync();
    }
}
