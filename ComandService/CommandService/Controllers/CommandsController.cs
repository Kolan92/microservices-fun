using AutoMapper;
using CommandService.Data;
using CommandService.Models;
using CommandService.PublicModels;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/platforms/{platformId:int}/[controller]")]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepository repository;
    private readonly IMapper mapper;

    public CommandsController(ICommandRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommandRead>>> GetCommandsForPlatform(int platformId)
    {
        var platformExists = await repository.PlatformExists(platformId);
        if (!platformExists)
        {
            return NotFound();
        }

        var commands = await repository.GetPlatformCommands(platformId);

        return Ok(mapper.Map<IEnumerable<CommandRead>>(commands));
    }

    [HttpGet("{commandId:int}", Name = "GetCommandForPlatform")]
    public async Task<ActionResult<CommandRead>> GetCommandForPlatform(int platformId, int commandId)
    {
        var platformExists = await repository.PlatformExists(platformId);
        if (!platformExists)
        {
            return NotFound();
        }

        var command = repository.GetCommand(platformId, commandId);

        if (command == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<CommandRead>(command));
    }

    [HttpPost]
    public async Task<ActionResult<CommandRead>> CreateCommandForPlatform(int platformId, CommandCreate commandCreate)
    {
        var platformExists = await repository.PlatformExists(platformId);
        if (!platformExists)
        {
            return NotFound();
        }

        var command = mapper.Map<Command>(commandCreate);

        await repository.CreateCommand(platformId, command);
        repository.SaveChanges();

        var commandReadDto = mapper.Map<CommandRead>(command);

        return CreatedAtRoute(nameof(GetCommandForPlatform),
            new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
    }
}