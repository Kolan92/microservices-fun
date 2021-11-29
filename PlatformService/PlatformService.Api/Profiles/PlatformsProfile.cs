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
        CreateMap<PlatformRead, PlatformPublished>();
        CreateMap<Platform, GrpcPlatformModel>()
            .ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher));
    }
}