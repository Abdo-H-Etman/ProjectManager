using Application.Features.Comments.DTOs;
using MediatR;

namespace Application.Features.Comments.Commands.CreateComment;

public record CreateCommentForTaskCommand(
    Guid TaskId,
    CreateCommentCommand Comment) : IRequest<CommentDto>;