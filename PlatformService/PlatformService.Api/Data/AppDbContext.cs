using Microsoft.EntityFrameworkCore;
using PlatformService.Api.Models;

namespace PlatformService.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Platform> Platforms { get; set; }
}