using MediatR;

namespace Application.Features.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(Guid Id) : IRequest;
