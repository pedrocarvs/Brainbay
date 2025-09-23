using System.ComponentModel.DataAnnotations;

namespace Brainbay.Core.Models;

public class Character
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}