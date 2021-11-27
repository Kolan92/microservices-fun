using System.ComponentModel.DataAnnotations;

namespace CommandService.PublicModels;

public class CommandCreate
{
    [Required]
    public string HowTo { get; set; }

    [Required]
    public string CommandLine { get; set; }
}