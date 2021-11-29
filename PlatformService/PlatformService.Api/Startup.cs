using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlatformService.Api.Data;
using PlatformService.Api.SyncDataServices.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PlatformService.Api.AsyncDataServices;
using PlatformService.Api.Configurations;
using PlatformService.Api.SyncDataServices.Grpc;

namespace PlatformService.Api;

public class Startup
{
    private readonly IConfiguration configuration;
    private const string kubernetesPrefix = "/platform-service";

    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("PlatformPostgresDb")));
        
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<ICommandDataClient, CommandDataClient>();
        services.AddSingleton<IMessageBusClient, MessageBusClient>();

        services.AddGrpc();
        
        services.AddOptions();
        services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMq"));
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddHealthChecks()
            .AddCheck("check", () => HealthCheckResult.Healthy("Health: OK"));
        
        services.AddHttpClient();
        services.AddControllers();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) 
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        
        app.UseSwagger(options => options.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            if (httpReq.Headers.ContainsKey("X-Forwarded-Prefix"))
            {
                swagger.Servers = new List<OpenApiServer> { new() { Url = kubernetesPrefix } };
            }
        }));

        app.UseSwaggerUI(c =>
        {
            if (env.IsDevelopment())
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Command Service V1");
            }
            else
            {
                c.SwaggerEndpoint($"{kubernetesPrefix}/swagger/v1/swagger.json", "Command Service V1");
            }
            c.RoutePrefix = string.Empty;
        });

        app.UseRouting();

        app.UseEndpoints(endpoints => 
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapControllers();

            endpoints.MapGrpcService<GrpcPlatformService>();
        });

        DatabaseSetup.SeedDatabase(app);
    }
}