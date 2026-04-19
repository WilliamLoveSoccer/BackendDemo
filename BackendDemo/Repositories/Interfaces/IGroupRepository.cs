using BackendDemo.Models;

namespace BackendDemo.Repositories;

public interface IGroupRepository
{
    /// <summary>
    /// Returns the subset of the provided group IDs that do not exist in the database.
    /// </summary>
    Task<IReadOnlyCollection<int>> GetMissingGroupIdsAsync(IEnumerable<int> groupIds);

    /// <summary>
    /// Stages a new group for insertion (caller must save).
    /// </summary>
    Task AddGroupAsync(Group group);

    /// <summary>
    /// Returns true if a group with the given ID exists.
    /// </summary>
    Task<bool> GroupExistsAsync(int id);

    /// <summary>
    /// Returns all groups.
    /// </summary>
    Task<IReadOnlyCollection<Group>> GetAllGroupsAsync();
}
