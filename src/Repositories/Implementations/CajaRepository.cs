using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class CajaRepository(AppDbContext db) : ICajaRepository
{
    public async Task<CajaApertura?> ObtenerAperturaActualAsync() =>
        await db.CajaAperturas
            .Include(c => c.Responsables)
                .ThenInclude(r => r.Empleado)
            .FirstOrDefaultAsync(c => c.CerradaEn == null);

    public async Task<CajaEstadoResponse> ObtenerEstadoActualAsync()
    {
        var apertura = await ObtenerAperturaActualAsync();
        if (apertura is null)
            return new CajaEstadoResponse(null, 0, 0, 0, 0, false);

        var totalIngresos = await db.Ingresos
            .Where(i => i.CajaAperturaId == apertura.Id)
            .SumAsync(i => i.Monto - i.Descuento);
        var totalEgresos = await db.Egresos
            .Where(e => e.CajaAperturaId == apertura.Id)
            .SumAsync(e => e.Monto);

        return new CajaEstadoResponse(
            apertura,
            apertura.MontoInicial,
            totalIngresos,
            totalEgresos,
            apertura.MontoInicial + totalIngresos - totalEgresos,
            true);
    }

    public async Task<PagedResult<CajaApertura>> ObtenerHistorialAsync(int page, int pageSize)
    {
        var q = db.CajaAperturas
            .Include(c => c.Responsables)
                .ThenInclude(r => r.Empleado)
            .Where(c => c.CerradaEn != null)
            .OrderByDescending(c => c.CerradaEn);
        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<CajaApertura>(items, total, page, pageSize);
    }

    public async Task<CajaApertura> AbrirAsync(CajaApertura apertura)
    {
        db.CajaAperturas.Add(apertura);
        await db.SaveChangesAsync();
        return apertura;
    }

    public async Task<CajaApertura?> CerrarAsync(int id, string? observaciones)
    {
        var apertura = await db.CajaAperturas.FindAsync(id);
        if (apertura is null || apertura.CerradaEn is not null) return null;
        var totalIngresos = await db.Ingresos
            .Where(i => i.CajaAperturaId == apertura.Id)
            .SumAsync(i => i.Monto - i.Descuento);
        var totalEgresos = await db.Egresos
            .Where(e => e.CajaAperturaId == apertura.Id)
            .SumAsync(e => e.Monto);
        apertura.CerradaEn = DateTime.UtcNow;
        apertura.TotalIngresos = totalIngresos;
        apertura.TotalEgresos = totalEgresos;
        apertura.SaldoFinal = apertura.MontoInicial + totalIngresos - totalEgresos;
        apertura.Observaciones = observaciones;
        await db.SaveChangesAsync();
        return apertura;
    }
}
