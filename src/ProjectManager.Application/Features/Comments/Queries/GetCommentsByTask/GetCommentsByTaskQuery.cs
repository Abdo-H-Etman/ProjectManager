using Application.Features.Comments.DTOs;
using MediatR;

namespace Application.Features.Comments.Queries.GetCommentsByTask;

public record GetCommentsByTaskQuery(Guid TaskId) : IRequest<IReadOnlyList<CommentDto>>;
