using Application.Features.Comments.DTOs;
using MediatR;

namespace Application.Features.Comments.Commands.CreateComment;

public record CreateCommentCommand : IRequest<CommentDto>
{
    public Guid TaskId { get; init; }
    public Guid? AuthorId { get; init; }
    public Guid? ParentCommentId { get; init; }
    public string Content { get; init; } = string.Empty;
}
