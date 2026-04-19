using System.ComponentModel.DataAnnotations;

namespace BackendDemo.DTOs;

public class CreateAlertRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;

    [Required, MinLength(1, ErrorMessage = "At least one group ID is required.")]
    public List<int> GroupIds { get; set; } = [];
}
