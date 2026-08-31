using Application.Common.Interfaces;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;

namespace Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(Task), request.Id);
        }

        await _unitOfWork.Tasks.DeleteAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
