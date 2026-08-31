using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(Task), request.Id);
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Priority = request.Priority;

        // If status changed to Completed and wasn't completed before, set CompletedAt
        if (request.Status == TaskStatus.Completed && task.Status != TaskStatus.Completed)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (request.Status != TaskStatus.Completed)
        {
            task.CompletedAt = null;
        }
        task.Status = request.Status;

        task.DueDate = request.DueDate;
        task.StartDate = request.StartDate;

        if (task.AssignedToId != request.AssignedToId)
        {
            task.AssignedToId = request.AssignedToId;
            task.AssignedAt = request.AssignedToId.HasValue ? DateTime.UtcNow : null;
        }

        task.EstimatedHours = request.EstimatedHours;
        task.ActualHours = request.ActualHours;
        task.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Tasks.UpdateAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
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
