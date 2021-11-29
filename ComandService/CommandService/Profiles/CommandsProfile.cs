using AutoMapper;
using CommandService.Models;
using CommandService.PublicModels;
using PlatformService;

namespace CommandService.Profiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        // Source -> Target
        CreateMap<Platform, PlatformRead>();
        CreateMap<CommandCreate, Command>();
        CreateMap<Command, CommandRead>();
        CreateMap<PlatformPublished, Platform>()
            .ForMember(
                dest => dest.ExternalId, 
                opt => opt.MapFrom(src => src.Id));

        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Commands, opt => opt.Ignore());
    }
}