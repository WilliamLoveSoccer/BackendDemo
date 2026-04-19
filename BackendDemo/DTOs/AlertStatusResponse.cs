namespace BackendDemo.DTOs;

public class AlertStatusResponse
{
    public int TotalRecipients { get; set; }
    public int SentCount { get; set; }
    public int FailedCount { get; set; }
    public int PendingCount { get; set; }
}
