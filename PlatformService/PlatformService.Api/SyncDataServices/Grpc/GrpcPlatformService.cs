using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using PlatformService.Api.Data;

namespace PlatformService.Api.SyncDataServices.Grpc;

public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
{
    private readonly IPlatformRepository repository;
    private readonly IMapper mapper;

    public GrpcPlatformService(IPlatformRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    public override async Task<PlatformResponse> GetAllPlatforms(GetAllPlatformsRequest request, ServerCallContext context)
    {
        var platforms = await repository.GetAllPlatforms();

        var response = new PlatformResponse();
        var grpcPlatforms = mapper.Map<IEnumerable<GrpcPlatformModel>>(platforms);
        response.Platforms.AddRange(grpcPlatforms);

        return response;
    }
}