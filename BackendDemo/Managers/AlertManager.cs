using BackendDemo.Data;
using BackendDemo.DTOs;
using BackendDemo.Models;
using BackendDemo.Repositories;

namespace BackendDemo.Managers;

public class AlertManager : IAlertManager
{
    #region Private Variables

    private readonly IAlertRepository _repository;
    private readonly IGroupRepository _groupRepository;
    private readonly IAppDbContext _db;

    #endregion

    #region Constructors

    public AlertManager(IAlertRepository repository, IGroupRepository groupRepository, IAppDbContext db)
    {
        _repository = repository;
        _groupRepository = groupRepository;
        _db = db;
    }

    #endregion

    #region Public Methods

    public async Task<Alert> CreateAlertAsync(CreateAlertRequest request)
    {
        var missingIds = await _groupRepository.GetMissingGroupIdsAsync(request.GroupIds);
        if (missingIds.Count > 0)
            throw new ApplicationException($"Groups not found: {string.Join(", ", missingIds)}.");

        var alert = new Alert
        {
            Title = request.Title,
            Body = request.Body,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
        };

        await _repository.AddAlertAsync(alert);

        var alertGroups = request.GroupIds.Select(gid => new AlertGroup { Alert = alert, GroupId = gid });
        await _repository.AddAlertGroupsAsync(alertGroups);

        await _db.SaveChangesAsync();

        return alert;
    }

    public async Task<AlertStatusResponse?> GetAlertStatusAsync(int id)
    {
        if (!await _repository.AlertExistsAsync(id))
            return null;

        var diliveryLogList = await _repository.GetDeliveryLogsAsync(log => log.AlertId == id);

        return new AlertStatusResponse
        {
            TotalRecipients = diliveryLogList.Select(l => l.UserId).Distinct().Count(),
            SentCount = diliveryLogList.Count(l => l.Status == DeliveryStatus.Sent),
            FailedCount = diliveryLogList.Count(l => l.Status == DeliveryStatus.Failed),
            PendingCount = diliveryLogList.Count(l => l.Status == DeliveryStatus.Pending),
        };
    }

    public async Task<PagedResult<AlertListItem>> GetAlertsAsync(int page, int pageSize)
    {
        var (items, total) = await _repository.GetPagenatedAlertsAsync(page, pageSize);
        return new PagedResult<AlertListItem>
        {
            Items = items.Select(a => new AlertListItem
            {
                Id = a.Id,
                Title = a.Title,
                CreatedBy = a.CreatedBy,
                CreatedAt = a.CreatedAt,
            }),
            Total = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    #endregion
}
