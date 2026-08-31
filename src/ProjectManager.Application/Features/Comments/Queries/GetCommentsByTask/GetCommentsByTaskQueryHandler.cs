using Application.Common.Interfaces;
using Application.Features.Comments.DTOs;
using MediatR;

namespace Application.Features.Comments.Queries.GetCommentsByTask;

public class GetCommentsByTaskQueryHandler : IRequestHandler<GetCommentsByTaskQuery, IReadOnlyList<CommentDto>>
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentsByTaskQueryHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<IReadOnlyList<CommentDto>> Handle(GetCommentsByTaskQuery request, CancellationToken cancellationToken)
    {
        var comments = await _commentRepository.GetCommentsByTaskIdAsync(request.TaskId, cancellationToken);

        return comments.Select(c => new CommentDto
        {
            Id = c.Id,
            TaskId = c.TaskId,
            AuthorId = c.AuthorId,
            ParentCommentId = c.ParentCommentId,
            Content = c.Content,
            IsEdited = c.IsEdited,
            EditedAt = c.EditedAt,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();
    }
}
