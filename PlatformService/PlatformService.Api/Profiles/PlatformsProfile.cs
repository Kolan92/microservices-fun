using AutoMapper;
using PlatformService.Api.Models;
using PlatformService.Api.PublicModels;

namespace PlatformService.Api.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<PlatformCreate, Platform>();
        CreateMap<Platform, PlatformRead>();
    }
}