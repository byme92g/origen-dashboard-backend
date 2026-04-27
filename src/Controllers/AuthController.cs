using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigenDashboard.Models;
using OrigenDashboard.Repositories.Interfaces;
using OrigenDashboard.Services;

namespace OrigenDashboard.Controllers;

[AllowAnonymous]
public class AuthController(IUsuarioRepository usuarios, ITokenService tokenService) : BaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var usuario = await usuarios.ObtenerPorUsuarioAsync(req.NombreUsuario);
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(req.Password, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        var token = tokenService.GenerarToken(usuario);

        return ApiOk(new
        {
            token,
            expiraEn = 1800,
            usuario = new
            {
                id = usuario.Id,
                nombreUsuario = usuario.NombreUsuario,
                nombreCompleto = usuario.NombreCompleto,
                rol = usuario.Rol
            }
        });
    }
}
