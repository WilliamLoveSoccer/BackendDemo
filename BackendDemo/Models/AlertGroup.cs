namespace BackendDemo.Models;

/// <summary>
/// Join entity linking an <see cref="Alert"/> to a <see cref="Group"/>.
/// </summary>
public class AlertGroup
{
    /// <summary>Foreign key to <see cref="Alert"/>.</summary>
    public int AlertId { get; set; }

    /// <summary>Navigation property to the associated alert.</summary>
    public Alert Alert { get; set; } = null!;

    /// <summary>Foreign key to <see cref="Group"/>.</summary>
    public int GroupId { get; set; }

    /// <summary>Navigation property to the associated group.</summary>
    public Group Group { get; set; } = null!;
}
