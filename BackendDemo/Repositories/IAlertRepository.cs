using System.Linq.Expressions;
using BackendDemo.DTOs;
using BackendDemo.Models;

namespace BackendDemo.Repositories;

/// <summary>
/// Data-access contract for alert persistence operations.
/// </summary>
public interface IAlertRepository
{
    /// <summary>
    /// Stages a new alert for insertion without saving.
    /// </summary>
    /// <param name="alert">The alert entity to add.</param>
    Task AddAlertAsync(Alert alert);

    /// <summary>
    /// Stages a collection of alert-group associations for insertion without saving.
    /// </summary>
    /// <param name="alertGroups">The <see cref="AlertGroup"/> join entities to add.</param>
    Task AddAlertGroupsAsync(IEnumerable<AlertGroup> alertGroups);

    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    Task SaveChangesAsync();

    /// <summary>
    /// Determines whether an alert with the given ID exists.
    /// </summary>
    /// <param name="id">The alert ID to check.</param>
    /// <returns><c>true</c> if the alert exists; otherwise <c>false</c>.</returns>
    Task<bool> AlertExistsAsync(int id);

    /// <summary>
    /// Returns delivery log entries that match the given predicate.
    /// </summary>
    /// <param name="predicate">Filter expression applied to <see cref="DeliveryLog"/> rows.</param>
    /// <returns>All matching <see cref="DeliveryLog"/> entries.</returns>
    Task<ICollection<DeliveryLog>> GetDeliveryLogsAsync(Expression<Func<DeliveryLog, bool>> predicate);

    /// <summary>
    /// Returns a paginated projection of alerts ordered by creation date descending.
    /// </summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>A tuple of the page items and the total record count.</returns>
    Task<(IEnumerable<AlertListItem> Items, int Total)> GetPagenatedAlertsAsync(int page, int pageSize);
}
