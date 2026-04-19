namespace BackendDemo.DTOs;

public class AlertListItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
