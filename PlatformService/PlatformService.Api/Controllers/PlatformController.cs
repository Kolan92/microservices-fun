using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Api.AsyncDataServices;
using PlatformService.Api.Data;
using PlatformService.Api.Models;
using PlatformService.Api.PublicModels;
using PlatformService.Api.SyncDataServices.Http;

namespace PlatformService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IPlatformRepository platformRepository;
    private readonly IMessageBusClient messageBusClient; 
    
    public PlatformController(IPlatformRepository platformRepository, IMapper mapper, IMessageBusClient messageBusClient)
    {
        this.platformRepository = platformRepository;
        this.mapper = mapper;
        this.messageBusClient = messageBusClient;
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
        if (platform == null) return NotFound();

        var model = mapper.Map<PlatformRead>(platform);
        return new OkObjectResult(model);
    }

    [HttpPost]
    public async Task<ActionResult<PlatformRead>> Create([Required] PlatformCreate platformCreate)
    {
        var platform = mapper.Map<Platform>(platformCreate);
        if (platform == null) return BadRequest();

        await platformRepository.CreatePlatform(platform);
        platformRepository.SaveChanges();

        var platformRead = mapper.Map<PlatformRead>(platform);
        messageBusClient.PublishNewPlatform(mapper.Map<PlatformPublished>(platformRead));
        
        return CreatedAtRoute(nameof(GetById), new { platformRead.Id }, platform);
    }
}