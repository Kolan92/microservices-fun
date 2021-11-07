using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PlatformService.Api;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

var app = builder.Build();
app.Run();