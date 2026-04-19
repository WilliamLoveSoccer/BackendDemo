using BackendDemo.DTOs;
using BackendDemo.Models;

namespace BackendDemo.Managers;

/// <summary>
/// Orchestrates alert business logic.
/// </summary>
public interface IAlertManager
{
    /// <summary>
    /// Creates a new alert and persists it.
    /// </summary>
    /// <param name="request">The alert creation payload.</param>
    /// <returns>The newly created <see cref="Alert"/>.</returns>
    Task<Alert> CreateAlertAsync(CreateAlertRequest request);

    /// <summary>
    /// Returns the delivery status summary for an alert, or <c>null</c> if not found.
    /// </summary>
    /// <param name="id">The alert ID.</param>
    /// <returns>An <see cref="AlertStatusResponse"/> with delivery counts, or <c>null</c>.</returns>
    Task<AlertStatusResponse?> GetAlertStatusAsync(int id);

    /// <summary>
    /// Returns a paginated list of alerts.
    /// </summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>A <see cref="PagedResult{AlertListItem}"/> for the requested page.</returns>
    Task<PagedResult<AlertListItem>> GetAlertsAsync(int page, int pageSize);
}
