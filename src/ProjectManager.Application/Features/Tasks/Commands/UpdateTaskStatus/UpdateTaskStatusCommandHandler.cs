using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(TaskEntity), request.Id);
        }

        var newStatus = Enum.Parse<TaskStatus>(request.Status, true);

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

        await _unitOfWork.Tasks.UpdateAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority.ToString(),
            Status = task.Status.ToString(),
            DueDate = task.DueDate,
            StartDate = task.StartDate,
            CompletedAt = task.CompletedAt,
            AssignedToId = task.AssignedToId,
            AssignedAt = task.AssignedAt,
            CreatedById = task.CreatedById,
            ParentTaskId = task.ParentTaskId,
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
