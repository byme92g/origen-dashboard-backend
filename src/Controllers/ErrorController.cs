using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OrigenDashboard.Controllers;

[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController(ILogger<ErrorController> logger) : ControllerBase
{
    private static readonly Dictionary<Type, int> ErrorStatusCodes = new()
    {
        [typeof(ArgumentException)] = StatusCodes.Status400BadRequest,
        [typeof(UnauthorizedAccessException)] = StatusCodes.Status401Unauthorized,
        [typeof(KeyNotFoundException)] = StatusCodes.Status404NotFound,
        [typeof(InvalidOperationException)] = StatusCodes.Status409Conflict,
    };

    [Route("/error")]
    public IActionResult Error()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var exception = feature?.Error;

        if (exception is not null && ErrorStatusCodes.TryGetValue(exception.GetType(), out var statusCode))
        {
            Response.StatusCode = statusCode;
            return new JsonResult(new { ok = false, error = exception.Message });
        }

        if (exception is not null)
            logger.LogError(exception, "Error no controlado en {Path}", feature?.Path);

        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new JsonResult(new { ok = false, error = "Error interno del servidor." });
    }
}
