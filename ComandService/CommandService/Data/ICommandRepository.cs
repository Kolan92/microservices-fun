using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;

public interface ICommandRepository
{
    void SaveChanges();

    Task<List<Platform>> GetAllPlatforms();
    ValueTask CreatePlatform(Platform platform);  
    ValueTask CreateMultiplePlatforms(IEnumerable<Platform> platform);

    Task<bool> PlatformExists(int platformId);
    Task<bool> ExternalPlatformExists(int externalPlatformId);


    Task<List<Command>> GetPlatformCommands(int platformId);
    Task<Command?> GetCommand(int platformId, int commandId);
    ValueTask CreateCommand(int platformId, Command command);
}

public class CommandRepository : ICommandRepository
{
    private readonly AppDbContext appDbContext;

    public CommandRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public void SaveChanges()
    {
        appDbContext.SaveChanges();
    }

    public Task<List<Platform>> GetAllPlatforms()
    {
        return appDbContext.Platforms.AsNoTracking().ToListAsync();
    }

    public async ValueTask CreatePlatform(Platform platform)
    {
        await appDbContext.Platforms.AddAsync(platform);
    }

    public async ValueTask CreateMultiplePlatforms(IEnumerable<Platform> platforms)
    {
        await appDbContext.Platforms.AddRangeAsync(platforms);
    }

    public Task<bool> ExternalPlatformExists(int externalPlatformId)
    {
        return appDbContext.Platforms.AnyAsync(platform => platform.ExternalId == externalPlatformId);
    }

    public Task<bool> PlatformExists(int platformId)
    {
        return appDbContext.Platforms.AnyAsync(platform => platform.Id == platformId);
    }

    public Task<List<Command>> GetPlatformCommands(int platformId)
    {
        return appDbContext.Commands
            .Where(command => command.Platform.Id == platformId)
            .ToListAsync();
    }

    public Task<Command?> GetCommand(int platformId, int commandId)
    {
        return appDbContext.Commands.SingleOrDefaultAsync(command => command.Platform.Id == platformId && command.Id == commandId);
    }

    public async ValueTask CreateCommand(int platformId, Command command)
    {
        var platformExists = await PlatformExists(platformId);
        if (!platformExists)
        {
            throw new AggregateException($"Platform with id: {platformId} doesn't exist");
        }

        command.PlatformId = platformId;
        await appDbContext.Commands.AddAsync(command);
    }
}