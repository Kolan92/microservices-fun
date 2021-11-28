namespace PlatformService.Api.PublicModels;

public class PlatformPublished
{
    public int Id { get; set; }

    public string Name { get; set; }
    
    public string Event => "PlatformPublished";
}