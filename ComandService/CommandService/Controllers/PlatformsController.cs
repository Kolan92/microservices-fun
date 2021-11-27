using AutoMapper;
using CommandService.Data;
using CommandService.PublicModels;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepository repository;
    private readonly IMapper mapper;

    public PlatformsController(ICommandRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformRead>>> GetPlatforms()
    {
        var platforms = await repository.GetAllPlatforms();
        var models = mapper.Map<IEnumerable<PlatformRead>>(platforms);

        return Ok(models);
    }
}