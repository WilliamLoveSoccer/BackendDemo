namespace BackendDemo.Models;

/// <summary>
/// Represents an alert that can be sent to one or more groups.
/// </summary>
public class Alert
{
    /// <summary>The primary key.</summary>
    public int Id { get; set; }

    /// <summary>Short display title of the alert.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Full message body of the alert.</summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the alert was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Identifier of the user who created the alert.</summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>Groups this alert is targeted at.</summary>
    public ICollection<AlertGroup> AlertGroups { get; set; } = [];

    /// <summary>Delivery log entries tracking per-recipient send status.</summary>
    public ICollection<DeliveryLog> DeliveryLogs { get; set; } = [];
}
