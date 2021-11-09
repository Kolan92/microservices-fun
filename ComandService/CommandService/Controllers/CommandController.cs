using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommandController : ControllerBase
{
    private readonly ILogger<CommandController> logger;

    public CommandController(ILogger<CommandController> logger)
    {
        this.logger = logger;
    }

    [HttpPost]
    public IActionResult Test()
    {
        var message = "Command controller test ok";
        logger.LogInformation(message);
        return Ok(message);
    }
}