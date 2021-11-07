using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommandController : ControllerBase
{

    [HttpPost]
    public IActionResult Test()
    {
        return Ok("Command controller test ok");
    }
}