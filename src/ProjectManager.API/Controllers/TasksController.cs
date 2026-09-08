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

    /// <summary>Gets unassigned backlog tasks.</summary>
    /// <param name="projectId">Optional project filter.</param>
    /// <returns>A list of unassigned tasks.</returns>
    [HttpGet("unassigned")]
    [ProducesResponseType(typeof(IReadOnlyList<TaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetUnassigned([FromQuery] Guid? projectId)
    {
        var result = await Mediator.Send(new Application.Features.Tasks.Queries.GetUnassignedTasks.GetUnassignedTasksQuery(projectId));
        return Ok(result);
    }

    /// <summary>Gets overdue tasks assigned to the current user.</summary>
    /// <returns>A list of overdue tasks.</returns>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(IReadOnlyList<TaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetOverdue()
    {
        var result = await Mediator.Send(new Application.Features.Tasks.Queries.GetOverdueTasks.GetOverdueTasksQuery());
        return Ok(result);
    }

    /// <summary>Updates the status of an existing task.</summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="request">The new status value.</param>
    /// <returns>The updated task.</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request)
    {
        var result = await Mediator.Send(new Application.Features.Tasks.Commands.UpdateTaskStatus.UpdateTaskStatusCommand(id, request.Status));
        return Ok(result);
    }

    /// <summary>Assigns or unassigns a task.</summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="request">The user ID to assign, or null to unassign.</param>
    /// <returns>The updated task.</returns>
    [HttpPatch("{id:guid}/assign")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> Assign(Guid id, [FromBody] AssignTaskRequest request)
    {
        var result = await Mediator.Send(new Application.Features.Tasks.Commands.AssignTask.AssignTaskCommand(id, request.AssignedToId));
        return Ok(result);
    }

    /// <summary>Gets subtasks for a task.</summary>
    /// <param name="id">The parent task identifier.</param>
    /// <returns>A list of subtasks.</returns>
    [HttpGet("{id:guid}/subtasks")]
    [ProducesResponseType(typeof(IReadOnlyList<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetSubtasks(Guid id)
    {
        var result = await Mediator.Send(new Application.Features.Tasks.Queries.GetSubtasks.GetSubtasksQuery(id));
        return Ok(result);
    }

    /// <summary>Creates a subtask under a parent task.</summary>
    /// <param name="id">The parent task identifier.</param>
    /// <param name="request">The subtask details.</param>
    /// <returns>The created subtask.</returns>
    [HttpPost("{id:guid}/subtasks")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> CreateSubtask(Guid id, [FromBody] CreateSubtaskRequest request)
    {
        var command = new Application.Features.Tasks.Commands.CreateSubtask.CreateSubtaskCommand(
            id,
            request.Title,
            request.Description,
            request.Priority,
            request.Status,
            request.DueDate,
            request.StartDate,
            request.AssignedToId,
            request.EstimatedHours);

        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}

/// <summary>Request payload for updating a task's status.</summary>
/// <param name="Status">The new status value.</param>
public record UpdateTaskStatusRequest(string Status);

/// <summary>Request payload for assigning a task.</summary>
/// <param name="AssignedToId">The assigned user ID or null to unassign.</param>
public record AssignTaskRequest(Guid? AssignedToId);

/// <summary>Request payload for creating a subtask.</summary>
/// <param name="Title">The subtask title.</param>
/// <param name="Description">The optional subtask description.</param>
/// <param name="Priority">The optional priority level.</param>
/// <param name="Status">The optional initial status.</param>
/// <param name="DueDate">The optional due date.</param>
/// <param name="StartDate">The optional start date.</param>
/// <param name="AssignedToId">The optional assignee user identifier.</param>
/// <param name="EstimatedHours">The optional estimated hours.</param>
public record CreateSubtaskRequest(
    string Title,
    string? Description = null,
    string? Priority = null,
    string? Status = null,
    DateTime? DueDate = null,
    DateTime? StartDate = null,
    Guid? AssignedToId = null,
    decimal? EstimatedHours = null);
