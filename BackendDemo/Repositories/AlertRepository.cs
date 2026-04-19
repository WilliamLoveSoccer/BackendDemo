using System.Linq.Expressions;
using BackendDemo.Data;
using BackendDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendDemo.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _db;

    public AlertRepository(AppDbContext db) => _db = db;

    public Task AddAlertAsync(Alert alert)
    {
        _db.Alerts.Add(alert);
        return Task.CompletedTask;
    }

    public Task AddAlertGroupsAsync(IEnumerable<AlertGroup> alertGroups)
    {
        _db.AlertGroups.AddRange(alertGroups);
        return Task.CompletedTask;
    }

    public Task<bool> AlertExistsAsync(int id) =>
        _db.Alerts.AnyAsync(a => a.Id == id);

    public async Task<ICollection<DeliveryLog>> GetDeliveryLogsAsync(Expression<Func<DeliveryLog, bool>> predicate)
    {
        return await _db.DeliveryLogs.Where(predicate).ToListAsync();
    }

    public async Task<(IEnumerable<Alert> Items, int Total)> GetPagenatedAlertsAsync(int page, int pageSize)
    {
        var query = _db.Alerts.OrderByDescending(a => a.CreatedAt);
        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
