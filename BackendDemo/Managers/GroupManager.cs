using BackendDemo.Data;
using BackendDemo.DTOs;
using BackendDemo.Models;
using BackendDemo.Repositories;

namespace BackendDemo.Managers;

public class GroupManager : IGroupManager
{
    #region Private Variables

    private readonly IGroupRepository _repository;
    private readonly IAppDbContext _db;

    #endregion

    #region Constructors

    public GroupManager(IGroupRepository repository, IAppDbContext db)
    {
        _repository = repository;
        _db = db;
    }

    #endregion

    #region Public Methods

    public async Task<Group> CreateGroupAsync(CreateGroupRequest request)
    {
        var group = new Group
        {
            Name = request.Name,
        };

        await _repository.AddGroupAsync(group);
        await _db.SaveChangesAsync();

        return group;
    }

    public async Task<IReadOnlyCollection<Group>> GetGroupsAsync()
    {
        return await _repository.GetAllGroupsAsync();
    }

    #endregion
}
