using OrigenDashboard.Models.Entities;

namespace OrigenDashboard.Services;

public interface ITokenService
{
    string GenerarToken(Usuario usuario);
}
