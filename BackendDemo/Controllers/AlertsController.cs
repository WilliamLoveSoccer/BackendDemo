using BackendDemo.DTOs;
using BackendDemo.Managers;
using Microsoft.AspNetCore.Mvc;

namespace BackendDemo.Controllers;

[ApiController]
[Route("alerts")]
public class AlertsController : ControllerBase
{
    #region Private Variables

    private readonly IAlertManager _manager;
    private readonly ILogger<AlertsController> _logger;

    #endregion

    #region Contructors

    public AlertsController(IAlertManager manager, ILogger<AlertsController> logger)
    {
        _manager = manager;
        _logger = logger;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a new alert.
    /// </summary>
    /// <param name="request">The alert creation payload.</param>
    /// <returns>201 Created with the new alert's ID, title, and timestamps; 400 if the request is invalid.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAlert([FromBody] CreateAlertRequest request)
    {
        try
        {
            var alert = await _manager.CreateAlertAsync(request);
            return CreatedAtAction(nameof(GetAlertStatus), new { id = alert.Id }, new { alert.Id, alert.Title, alert.CreatedAt, alert.CreatedBy });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating alert.");
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }

    /// <summary>
    /// Returns the current status of a single alert.
    /// </summary>
    /// <param name="id">The alert ID.</param>
    /// <returns>200 with the status object, or 404 if the alert does not exist.</returns>
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetAlertStatus(int id)
    {
        try
        {
            var status = await _manager.GetAlertStatusAsync(id);
            if (status is null)
                return NotFound(new { error = $"Alert {id} not found." });

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving status for alert {AlertId}.", id);
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }

    /// <summary>
    /// Returns a paginated list of alerts.
    /// </summary>
    /// <param name="page">1-based page number (default 1).</param>
    /// <param name="pageSize">Number of items per page, between 1 and 100 (default 10).</param>
    /// <returns>200 with the paged result, or 400 if pagination parameters are out of range.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAlerts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1)
            return BadRequest(new { error = "Page must be >= 1." });

        if (pageSize < 1 || pageSize > 100)
            return BadRequest(new { error = "PageSize must be between 1 and 100." });

        try
        {
            var result = await _manager.GetAlertsAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving alerts (page {Page}, pageSize {PageSize}).", page, pageSize);
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }

    #endregion
}
