using Application.Features.Comments.DTOs;
using MediatR;

namespace Application.Features.Comments.Commands.UpdateComment;

public record UpdateCommentCommand(Guid Id, string Content) : IRequest<CommentDto>;
