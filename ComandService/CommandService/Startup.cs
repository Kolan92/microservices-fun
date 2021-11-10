using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;

namespace CommandService;

public class Startup
{
    private const string kubernetesPrefix = "/command-service";
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
            endpoints.MapControllers();
        });
    }
}