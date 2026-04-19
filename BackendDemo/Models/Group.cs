namespace BackendDemo.Models;

/// <summary>
/// Represents a group of recipients that alerts can be targeted at.
/// </summary>
public class Group
{
    /// <summary>The primary key.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the group.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Alerts associated with this group.</summary>
    public ICollection<AlertGroup> AlertGroups { get; set; } = [];
}
