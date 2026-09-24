using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateTaskStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TaskDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(TaskEntity), request.Id);
        }

        var newStatus = Enum.Parse<TaskStatus>(request.Status, true);

        if (!task.Status.CanTransitionTo(newStatus))
        {
            throw new InvalidTaskStatusTransitionException(task.Status, newStatus);
        }

        if (newStatus == TaskStatus.Completed && task.Status != TaskStatus.Completed)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (newStatus != TaskStatus.Completed)
        {
            task.CompletedAt = null;
        }

        task.Status = newStatus;
        task.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(task);
    }

}
