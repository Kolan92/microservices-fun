using CommandService.Configurations;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandService.SyncDataService.Grpc;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CommandService;

public class Startup
{
    private readonly IConfiguration configuration;
    private const string kubernetesPrefix = "/command-service";
    
    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("CommandPostgresDb")));
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddHealthChecks()
            .AddCheck("check", () => HealthCheckResult.Healthy("Health: OK"));
        
        services.AddScoped<ICommandRepository, CommandRepository>();
        services.AddScoped<IEventProcessor, EventProcessor>();
        services.AddScoped<IPlatformDataService, PlatformDataService>();
        services.AddHostedService<MessageBusEventHandler>();
        services.AddHostedService<PlatformInitiator>();
        
        services.AddOptions();
        services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMq"));
        
        services.AddControllers();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CommandService", Version = "v1" });
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
        });
        
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
        
        dbContext.Database.Migrate();
    }
}