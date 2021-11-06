using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Api.Data;
using PlatformService.Api.Models;
using PlatformService.Api.PublicModels;

namespace PlatformService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{

    private readonly IPlatformRepository platformRepository;
    private readonly IMapper mapper;

    public PlatformController(IPlatformRepository platformRepository, IMapper mapper)
    {
        this.platformRepository = platformRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var platforms = await platformRepository.GetAllPlatforms();
        var models = mapper.Map<IEnumerable<PlatformRead>>(platforms);

        return new OkObjectResult(models);
    }

    [HttpGet("{id:int}", Name = "GetById")]
    public async Task<IActionResult> GetById(int id)
    {
        var platform = await platformRepository.GetPlatformById(id);
        if (platform == null)
        {
            return NotFound();
        }

        var model = mapper.Map<PlatformRead>(platform);
        return new OkObjectResult(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([Required] PlatformCreate platformCreate)
    {
        var platform = mapper.Map<Platform>(platformCreate);
        if (platform == null)
        {
            return BadRequest();
        }

        await platformRepository.CreatePlatform(platform);
        platformRepository.SaveChanges();

        var platformRead = mapper.Map<PlatformRead>(platform);
        return CreatedAtRoute(nameof(GetById), new {Id= platformRead.Id}, platform);
    }
}