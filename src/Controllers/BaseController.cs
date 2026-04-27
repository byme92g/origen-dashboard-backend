using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrigenDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class BaseController : ControllerBase
{
    protected IActionResult ApiOk<T>(T data) => Ok(new { ok = true, data });
    protected IActionResult ApiCreated<T>(T data) => StatusCode(201, new { ok = true, data });
    protected IActionResult ApiOk() => Ok(new { ok = true });
}
