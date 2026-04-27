using OrigenDashboard.Models;
using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Repositories.Interfaces;

public interface ICajaRepository
{
    Task<CajaApertura?> ObtenerAperturaActualAsync();
    Task<CajaEstadoResponse> ObtenerEstadoActualAsync();
    Task<PagedResult<CajaApertura>> ObtenerHistorialAsync(int page, int pageSize);
    Task<CajaApertura> AbrirAsync(CajaApertura apertura);
    Task<CajaApertura?> CerrarAsync(int id, string? observaciones);
}
