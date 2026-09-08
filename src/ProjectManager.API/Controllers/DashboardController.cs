using Application.Features.Dashboard.DTOs;
using Application.Features.Dashboard.Queries.GetMyDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.API.Controllers;

/// <summary>
/// Provides endpoints for retrieving user and workspace dashboards.
/// </summary>
[Authorize]
public class DashboardController : ApiControllerBase
{
    /// <summary>Gets the current authenticated user's workspace dashboard metrics.</summary>
    /// <returns>Dashboard summary metrics and upcoming deadlines.</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DashboardDto>> GetMyDashboard()
    {
        var result = await Mediator.Send(new GetMyDashboardQuery());
        return Ok(result);
    }
}
