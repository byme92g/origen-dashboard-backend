using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class ClienteRepository(AppDbContext db) : IClienteRepository
{
    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync() =>
        await db.Clientes.OrderBy(c => c.Nombre).ToListAsync();

    public async Task<PagedResult<Cliente>> ObtenerPaginadoAsync(int page, int pageSize)
    {
        var q = db.Clientes.OrderBy(c => c.Nombre);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Cliente>(items, total, page, pageSize);
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id) =>
        await db.Clientes.FindAsync(id);

    public async Task<Cliente> CrearAsync(Cliente cliente)
    {
        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();
        return cliente;
    }

    public async Task<bool> ActualizarAsync(Cliente cliente)
    {
        db.Clientes.Update(cliente);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is null) return false;
        db.Clientes.Remove(cliente);
        return await db.SaveChangesAsync() > 0;
    }
}
