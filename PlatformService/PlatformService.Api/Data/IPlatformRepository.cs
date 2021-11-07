using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlatformService.Api.Models;

namespace PlatformService.Api.Data;

public interface IPlatformRepository
{
    bool SaveChanges();

    Task<List<Platform>> GetAllPlatforms();

    Task<Platform?> GetPlatformById(int id);

    ValueTask CreatePlatform(Platform platform);
}

public class PlatformRepository : IPlatformRepository
{
    private readonly AppDbContext appDbContext;

    public PlatformRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public bool SaveChanges()
    {
        var changedRows = appDbContext.SaveChanges();
        return changedRows >= 0;
    }

    public Task<List<Platform>> GetAllPlatforms()
    {
        return appDbContext.Platforms.AsNoTracking().ToListAsync();
    }

    public Task<Platform?> GetPlatformById(int id)
    {
        return appDbContext.Platforms.AsNoTracking().FirstOrDefaultAsync(platform => platform.Id == id);
    }

    public async ValueTask CreatePlatform(Platform platform)
    {
        if (platform == null) throw new ArgumentNullException(nameof(platform));
        await appDbContext.Platforms.AddAsync(platform);
    }
}