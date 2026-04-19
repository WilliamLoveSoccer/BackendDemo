using BackendDemo.Data;
using BackendDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendDemo.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly AppDbContext _db;

    public GroupRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<int>> GetMissingGroupIdsAsync(IEnumerable<int> groupIds)
    {
        var ids = groupIds.Distinct().ToList();
        var existingIds = await _db.Groups.Where(g => ids.Contains(g.Id)).Select(g => g.Id).ToListAsync();
        return [.. ids.Except(existingIds)];
    }

    public async Task AddGroupAsync(Group group)
    {
        await _db.Groups.AddAsync(group);
    }

    public async Task<bool> GroupExistsAsync(int id)
    {
        return await _db.Groups.AnyAsync(g => g.Id == id);
    }

    public async Task<IReadOnlyCollection<Group>> GetAllGroupsAsync()
    {
        return await _db.Groups.OrderBy(g => g.Name).ToListAsync();
    }
}
