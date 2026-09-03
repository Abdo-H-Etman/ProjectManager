using Application.Features.Tasks.Commands.CreateTask;
using Application.Features.Tasks.Commands.DeleteTask;
using Application.Features.Tasks.Commands.UpdateTask;
using Application.Features.Tasks.DTOs;
using Application.Features.Tasks.Queries.GetTaskById;
using Application.Features.Tasks.Queries.GetTasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.API.Controllers;

/// <summary>
/// Provides endpoints for managing tasks.
/// </summary>
[Authorize]
public class TasksController : ApiControllerBase
{
    /// <summary>Creates a task in a project.</summary>
    /// <param name="command">The task details.</param>
    /// <returns>The newly created task.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Gets tasks visible to the current user.</summary>
    /// <param name="query">Optional filters and pagination settings.</param>
    /// <returns>A list of tasks.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetAll([FromQuery] GetTasksQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>Gets a task by its identifier.</summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task and its details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDetailDto>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetTaskByIdQuery(id));
        return Ok(result);
    }

    /// <summary>Updates an existing task.</summary>
    /// <param name="id">The task identifier in the route.</param>
    /// <param name="command">The updated task details, including the matching identifier.</param>
    /// <returns>The updated task.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> Update(Guid id, [FromBody] UpdateTaskCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "Id in route does not match Id in body." });
        }

        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>Deletes a task.</summary>
    /// <param name="id">The task identifier.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteTaskCommand(id));
        return NoContent();
    }
}
