using System.Text.Json;
using System.Text.Json.Nodes;
using AutoMapper;
using CommandService.Data;
using CommandService.Models;
using CommandService.PublicModels;

namespace CommandService.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}

public enum EventType
{
    PlatformPublished,
    Unknown
}

public class EventProcessor : IEventProcessor
{
    private readonly ICommandRepository commandRepository;
    private readonly IMapper mapper;
    private readonly ILogger<EventProcessor> logger;

    public EventProcessor(ICommandRepository commandRepository, IMapper mapper, ILogger<EventProcessor> logger)
    {
        this.commandRepository = commandRepository;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task ProcessEvent(string message)
    {
        var baseEvent = JsonSerializer.Deserialize<JsonObject>(message);
        var eventType = Enum.Parse<EventType>(baseEvent["Event"].GetValue<string>());

        switch (eventType)
        {
            case EventType.PlatformPublished:
                var platformPublished = baseEvent.Deserialize<PlatformPublished>();
                var newPlatform = mapper.Map<Platform>(platformPublished);
                await AddPlatform(newPlatform);
                break;
            default:
                logger.LogWarning($"Received unknown event: {message}");
                break;
        }
    }

    private async Task AddPlatform(Platform newPlatform)
    {
        var platformExists = await commandRepository.ExternalPlatformExists(newPlatform.ExternalId);
        if (platformExists)
        {
            logger.LogWarning($"Platform with external id: {newPlatform.ExternalId} already exists");
            return;
        }

        await commandRepository.CreatePlatform(newPlatform);
        commandRepository.SaveChanges();
        
        logger.LogInformation($"Successfully added new platform with external id: {newPlatform.ExternalId}");
    }
}