using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PlatformService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        var app = builder.Build();
        app.Run();
    }
}