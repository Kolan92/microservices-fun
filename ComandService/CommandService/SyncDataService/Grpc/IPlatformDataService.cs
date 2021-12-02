using AutoMapper;
using CommandService.Models;
using Grpc.Core;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataService.Grpc;

public interface IPlatformDataService
{
    Task<IEnumerable<Platform>> GetAllPlatforms();
}

public class PlatformDataService : IPlatformDataService
{
    private readonly IMapper mapper;
    private readonly string url;

    public PlatformDataService(IConfiguration configuration, IMapper mapper)
    {
        url = configuration["GrpcPlatform"];
        this.mapper = mapper;
    }

    public async Task<IEnumerable<Platform>> GetAllPlatforms()
    {
        var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        };

        using var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions { HttpHandler = httpHandler , Credentials = ChannelCredentials.Insecure });
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllPlatformsRequest();

        var response = await client.GetAllPlatformsAsync(request);
        return mapper.Map<IEnumerable<Platform>>(response.Platforms);
    }
}