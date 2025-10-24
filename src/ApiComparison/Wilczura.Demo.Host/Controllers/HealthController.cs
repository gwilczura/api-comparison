using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wilczura.Demo.Common;

namespace Wilczura.Demo.Host.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    public HealthController()
    {
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetAsync()
    {
        await Task.CompletedTask;
        return Ok($"{SystemInfo.GetInfo("demo")} | Health");
    }
}
