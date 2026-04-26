using Microsoft.EntityFrameworkCore;
using OrigenDashboard.Data;
using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;
using OrigenDashboard.Repositories.Interfaces;

namespace OrigenDashboard.Repositories.Implementations;

public class CajaRepository(AppDbContext db) : ICajaRepository
{
    public async Task<CajaApertura?> ObtenerAperturaActualAsync() =>
        await db.CajaAperturas.FirstOrDefaultAsync(c => c.CerradaEn == null);

    public async Task<PagedResult<CajaApertura>> ObtenerHistorialAsync(int page, int pageSize)
    {
        var q = db.CajaAperturas
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

    public async Task<bool> CerrarAsync(int id, decimal totalIngresos, decimal totalEgresos, decimal saldoFinal, string? observaciones)
    {
        var apertura = await db.CajaAperturas.FindAsync(id);
        if (apertura is null || apertura.CerradaEn is not null) return false;
        apertura.CerradaEn = DateTime.UtcNow;
        apertura.TotalIngresos = totalIngresos;
        apertura.TotalEgresos = totalEgresos;
        apertura.SaldoFinal = saldoFinal;
        apertura.Observaciones = observaciones;
        return await db.SaveChangesAsync() > 0;
    }
}
