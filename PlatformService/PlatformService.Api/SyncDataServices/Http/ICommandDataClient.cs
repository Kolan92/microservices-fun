using System.Threading.Tasks;
using PlatformService.Api.PublicModels;

namespace PlatformService.Api.SyncDataServices.Http;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformRead platform);
}