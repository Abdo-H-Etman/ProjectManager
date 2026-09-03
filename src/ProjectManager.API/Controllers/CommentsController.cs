using Application.Features.Comments.Commands.CreateComment;
using Application.Features.Comments.Commands.DeleteComment;
using Application.Features.Comments.DTOs;
using Application.Features.Comments.Queries.GetCommentsByTask;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.API.Controllers;

/// <summary>
/// Provides endpoints for managing task comments.
/// </summary>
[Authorize]
public class CommentsController : ApiControllerBase
{
    /// <summary>Creates a comment for a task.</summary>
    /// <param name="taskId">The task that will receive the comment.</param>
    /// <param name="command">The comment content.</param>
    /// <returns>The newly created comment.</returns>
    [HttpPost("~/api/tasks/{taskId:guid}/comments")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> Create(Guid taskId, [FromBody] CreateCommentCommand command)
    {
        var result = await Mediator.Send(new CreateCommentForTaskCommand(taskId, command));
        return Created($"api/tasks/{taskId}/comments/{result.Id}", result);
    }

    /// <summary>Gets all comments belonging to a task.</summary>
    /// <param name="taskId">The task whose comments should be returned.</param>
    /// <returns>The task's comments.</returns>
    [HttpGet("~/api/tasks/{taskId:guid}/comments")]
    [ProducesResponseType(typeof(IReadOnlyList<CommentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> GetByTaskId(Guid taskId)
    {
        var result = await Mediator.Send(new GetCommentsByTaskQuery(taskId));
        return Ok(result);
    }

    /// <summary>Deletes a comment.</summary>
    /// <param name="id">The comment identifier.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteCommentCommand(id));
        return NoContent();
    }
}
