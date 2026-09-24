using Application.Common.Interfaces;
using Application.Common.Authorization;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;

namespace Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(Task), request.Id);
        }

        AuthorizationRules.RequireOwnerOrAdmin(_currentUserService, task.Project.OwnerId, "You can only delete tasks from your own projects.");

        if (task.IsDeleted)
        {
            throw new DeletedException(nameof(Task), request.Id);
        }

        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;
        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
