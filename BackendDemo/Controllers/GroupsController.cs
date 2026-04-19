using BackendDemo.DTOs;
using BackendDemo.Managers;
using Microsoft.AspNetCore.Mvc;

namespace BackendDemo.Controllers;

[ApiController]
[Route("v1/groups")]
public class GroupsController : ControllerBase
{
    #region Private Variables

    private readonly IGroupManager _manager;
    private readonly ILogger<GroupsController> _logger;

    #endregion

    #region Constructors

    public GroupsController(IGroupManager manager, ILogger<GroupsController> logger)
    {
        _manager = manager;
        _logger = logger;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns all groups.
    /// </summary>
    /// <returns>200 with the list of groups.</returns>
    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        try
        {
            var groups = await _manager.GetGroupsAsync();
            return Ok(groups.Select(g => new { g.Id, g.Name }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving groups.");
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }

    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="request">The group creation payload.</param>
    /// <returns>201 Created with the new group's ID and name; 400 if the request is invalid.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        try
        {
            var group = await _manager.CreateGroupAsync(request);
            return CreatedAtAction(nameof(CreateGroup), new { id = group.Id }, new { group.Id, group.Name });
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating group.");
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }

    #endregion
}
