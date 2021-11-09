using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using PlatformService.Api.PublicModels;

namespace PlatformService.Api.SyncDataServices.Http;

public class CommandDataClient : ICommandDataClient
{
    private readonly HttpClient httpClient;
    private readonly string commandServiceUrl;

    public CommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = Guard.Against.Null(httpClient, nameof(httpClient));
        
        commandServiceUrl = Guard.Against.NullOrWhiteSpace(configuration?["CommandServiceUrl"], "CommandServiceUrl");
    }

    public async Task SendPlatformToCommand(PlatformRead platform)
    {
        var response = await httpClient.PostAsJsonAsync($"{commandServiceUrl}/command-service/api/Command", platform);
        response.EnsureSuccessStatusCode();
    }
}