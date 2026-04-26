using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface ICajaRepository
{
    Task<CajaApertura?> ObtenerAperturaActualAsync();
    Task<PagedResult<CajaApertura>> ObtenerHistorialAsync(int page, int pageSize);
    Task<CajaApertura> AbrirAsync(CajaApertura apertura);
    Task<bool> CerrarAsync(int id, decimal totalIngresos, decimal totalEgresos, decimal saldoFinal, string? observaciones);
}
