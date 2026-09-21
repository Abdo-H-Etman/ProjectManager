using Application.Common.Interfaces;
using Application.Features.Comments.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;

namespace Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentForTaskCommand, CommentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateCommentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CommentDto> Handle(CreateCommentForTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(TaskEntity), request.TaskId);
        }

        if (task.IsDeleted)
        {
            throw new DeletedException(nameof(TaskEntity), request.TaskId);
        }

        var authorId = _currentUserService.UserId ?? Guid.NewGuid();

        var comment = new Comment
        {
            TaskId = request.TaskId,
            AuthorId = authorId,
            ParentCommentId = request.Comment.ParentCommentId,
            Content = request.Comment.Content,
            IsEdited = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Comments.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CommentDto
        {
            Id = comment.Id,
            TaskId = comment.TaskId,
            AuthorId = comment.AuthorId,
            ParentCommentId = comment.ParentCommentId,
            Content = comment.Content,
            IsEdited = comment.IsEdited,
            EditedAt = comment.EditedAt,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}
