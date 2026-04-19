namespace BackendDemo.Models;

/// <summary>
/// Records the delivery status of an <see cref="Alert"/> for a single recipient.
/// </summary>
public class DeliveryLog
{
    /// <summary>The primary key.</summary>
    public int Id { get; set; }

    /// <summary>Foreign key to the associated <see cref="Alert"/>.</summary>
    public int AlertId { get; set; }

    /// <summary>Navigation property to the associated alert.</summary>
    public Alert Alert { get; set; } = null!;

    /// <summary>Identifier of the recipient user.</summary>
    public int UserId { get; set; }

    /// <summary>Current delivery status for this recipient.</summary>
    public DeliveryStatus Status { get; set; }

    /// <summary>UTC timestamp of the last status update.</summary>
    public DateTime Timestamp { get; set; }
}
