using System.ComponentModel.DataAnnotations;

namespace BackendDemo.DTOs;

public class CreateGroupRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
