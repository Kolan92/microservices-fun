using AutoMapper;
using CommandService.Models;
using CommandService.PublicModels;

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
    }
}