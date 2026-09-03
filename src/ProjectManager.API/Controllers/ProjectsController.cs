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
}
