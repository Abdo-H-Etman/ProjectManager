using Application.Features.Projects.Commands.CreateProject;
using Application.Features.Projects.Commands.DeleteProject;
using Application.Features.Projects.Commands.UpdateProject;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries.GetProjectById;
using Application.Features.Projects.Queries.GetProjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.API.Controllers;

/// <summary>
/// Provides endpoints for managing projects.
/// </summary>
[Authorize]
public class ProjectsController : ApiControllerBase
{
    /// <summary>Creates a project owned by the current user.</summary>
    /// <param name="command">The project details.</param>
    /// <returns>The newly created project.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Gets projects visible to the current user.</summary>
    /// <param name="query">Optional filters and pagination settings.</param>
    /// <returns>A list of projects.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll([FromQuery] GetProjectsQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>Gets a project by its identifier.</summary>
    /// <param name="id">The project identifier.</param>
    /// <returns>The project and its details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetailDto>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetProjectByIdQuery(id));
        return Ok(result);
    }

    /// <summary>Updates an existing project.</summary>
    /// <param name="id">The project identifier in the route.</param>
    /// <param name="command">The updated project details, including the matching identifier.</param>
    /// <returns>The updated project.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> Update(Guid id, [FromBody] UpdateProjectCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "Id in route does not match Id in body." });
        }

        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>Deletes a project.</summary>
    /// <param name="id">The project identifier.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteProjectCommand(id));
        return NoContent();
    }

    /// <summary>Gets all tasks for a project.</summary>
    /// <param name="id">The project identifier.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="priority">Optional priority filter.</param>
    /// <param name="assignedToId">Optional assignee filter.</param>
    /// <returns>A list of tasks in the project.</returns>
    [HttpGet("{id:guid}/tasks")]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Tasks.DTOs.TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<Application.Features.Tasks.DTOs.TaskDto>>> GetTasks(
        Guid id,
        [FromQuery] string? status = null,
        [FromQuery] string? priority = null,
        [FromQuery] Guid? assignedToId = null)
    {
        var result = await Mediator.Send(new Application.Features.Tasks.Queries.GetTasksByProject.GetTasksByProjectQuery(id, status, priority, assignedToId));
        return Ok(result);
    }

    /// <summary>Gets statistics and summary metrics for a project.</summary>
    /// <param name="id">The project identifier.</param>
    /// <returns>The project metrics summary.</returns>
    [HttpGet("{id:guid}/summary")]
    [ProducesResponseType(typeof(ProjectSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectSummaryDto>> GetSummary(Guid id)
    {
        var result = await Mediator.Send(new Application.Features.Projects.Queries.GetProjectSummary.GetProjectSummaryQuery(id));
        return Ok(result);
    }

    /// <summary>Archives or unarchives a project.</summary>
    /// <param name="id">The project identifier.</param>
    /// <param name="request">Archive status payload.</param>
    /// <returns>The updated project.</returns>
    [HttpPatch("{id:guid}/archive")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> Archive(Guid id, [FromBody] ArchiveProjectRequest? request)
    {
        var isArchived = request?.IsArchived ?? true;
        var result = await Mediator.Send(new Application.Features.Projects.Commands.ArchiveProject.ArchiveProjectCommand(id, isArchived));
        return Ok(result);
    }
}

/// <summary>Request payload for archiving or unarchiving a project.</summary>
/// <param name="IsArchived">Whether the project should be archived.</param>
public record ArchiveProjectRequest(bool IsArchived = true);
