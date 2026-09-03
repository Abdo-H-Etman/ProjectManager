using Application.Features.Comments.Commands.CreateComment;
using Application.Features.Comments.Commands.DeleteComment;
using Application.Features.Comments.DTOs;
using Application.Features.Comments.Queries.GetCommentsByTask;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManager.API.Controllers;

[Authorize]
public class CommentsController : ApiControllerBase
{
    [HttpPost("~/api/tasks/{taskId:guid}/comments")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> Create(Guid taskId, [FromBody] CreateCommentCommand command)
    {
        var result = await Mediator.Send(new CreateCommentForTaskCommand(taskId, command));
        return Created($"api/tasks/{taskId}/comments/{result.Id}", result);
    }

    [HttpGet("~/api/tasks/{taskId:guid}/comments")]
    [ProducesResponseType(typeof(IReadOnlyList<CommentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> GetByTaskId(Guid taskId)
    {
        var result = await Mediator.Send(new GetCommentsByTaskQuery(taskId));
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteCommentCommand(id));
        return NoContent();
    }
}
