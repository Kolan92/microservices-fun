using System.ComponentModel.DataAnnotations;

namespace PlatformService.Api.PublicModels;

public class PlatformRead
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Publisher { get; set; }

    public string Cost { get; set; }
}

public class PlatformCreate
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Publisher { get; set; }

    [Required]
    public string Cost { get; set; }
}