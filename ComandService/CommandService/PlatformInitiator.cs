using CommandService.Data;
using CommandService.SyncDataService.Grpc;

namespace CommandService;

public class PlatformInitiator : IHostedService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<MessageBusEventHandler> logger;

    public PlatformInitiator(IServiceScopeFactory scopeFactory, ILogger<MessageBusEventHandler> logger)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var platformClient = scope.ServiceProvider.GetRequiredService<IPlatformDataService>();
        var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
        var externalPlatformsTask = platformClient.GetAllPlatforms();
        var existingPlatformsTask = repository.GetAllPlatforms();

        var (externalPlatforms, existingPlatforms) = await TaskExtensions.WhenAll(externalPlatformsTask, existingPlatformsTask);

        var newPlatforms = externalPlatforms.Where(externalPlatform => existingPlatforms.All(existingPlatform => existingPlatform.ExternalId != externalPlatform.ExternalId))
            .ToList();

        await repository.CreateMultiplePlatforms(newPlatforms);
        repository.SaveChanges();
        
        logger.LogInformation($"Successfully synchronized {newPlatforms.Count} from platforms service");
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        //Do nothing
        return Task.CompletedTask;
    }
}