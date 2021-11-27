using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Api.Models;

namespace PlatformService.Api.Data;

public static class DatabaseSetup
{
    public static void SeedDatabase(IApplicationBuilder applicationBuilder)
    {
        using var scope = applicationBuilder.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
        
        dbContext.Database.Migrate();

        if (dbContext.Platforms.Any())
        {
            Console.WriteLine("Database already contains data");
            Console.WriteLine("No data is seeded to database");
            return;
        }

        Console.WriteLine("Seeding database");

        dbContext.Platforms.AddRange(
            new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
            new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
            new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
        );

        dbContext.SaveChanges();
    }
}