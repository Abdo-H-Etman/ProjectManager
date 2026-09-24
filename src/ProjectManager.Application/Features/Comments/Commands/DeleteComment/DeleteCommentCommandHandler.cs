using Application.Common.Interfaces;
using Application.Common.Authorization;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Comments.Commands.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCommentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async System.Threading.Tasks.Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(request.Id, cancellationToken);
        if (comment == null)
        {
            throw new NotFoundException(nameof(Comment), request.Id);
        }

        AuthorizationRules.RequireOwnerOrAdmin(_currentUserService, comment.AuthorId,
            "You can only delete your own comments.");

        _unitOfWork.Comments.Delete(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
