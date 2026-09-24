using Application.Common.Interfaces;
using Application.Common.Authorization;
using Application.Features.Comments.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Comments.Commands.UpdateComment;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, CommentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCommentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CommentDto> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(request.Id, cancellationToken);
        if (comment == null)
        {
            throw new NotFoundException(nameof(Comment), request.Id);
        }

        AuthorizationRules.RequireOwner(_currentUserService, comment.AuthorId, "You can only update your own comments.");

        comment.Content = request.Content;
        comment.IsEdited = true;
        comment.EditedAt = DateTime.UtcNow;
        comment.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Comments.Update(comment);
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
