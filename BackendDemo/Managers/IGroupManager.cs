using BackendDemo.DTOs;
using BackendDemo.Models;

namespace BackendDemo.Managers;

/// <summary>
/// Orchestrates group business logic.
/// </summary>
public interface IGroupManager
{
    /// <summary>
    /// Creates a new group and persists it.
    /// </summary>
    /// <param name="request">The group creation payload.</param>
    /// <returns>The newly created <see cref="Group"/>.</returns>
    Task<Group> CreateGroupAsync(CreateGroupRequest request);

    /// <summary>
    /// Returns all groups.
    /// </summary>
    Task<IReadOnlyCollection<Group>> GetGroupsAsync();
}
