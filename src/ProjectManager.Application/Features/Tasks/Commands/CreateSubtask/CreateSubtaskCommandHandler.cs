using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;
using TaskPriority = Domain.Enums.TaskPriority;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.CreateSubtask;

public class CreateSubtaskCommandHandler : IRequestHandler<CreateSubtaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateSubtaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<TaskDto> Handle(CreateSubtaskCommand request, CancellationToken cancellationToken)
    {
        var parentTask = await _unitOfWork.Tasks.GetByIdAsync(request.ParentTaskId, cancellationToken);
        if (parentTask == null)
        {
            throw new NotFoundException(nameof(TaskEntity), request.ParentTaskId);
        }

        var priority = Enum.TryParse<TaskPriority>(request.Priority, true, out var parsedPriority)
            ? parsedPriority
            : TaskPriority.Medium;

        var status = Enum.TryParse<TaskStatus>(request.Status, true, out var parsedStatus)
            ? parsedStatus
            : TaskStatus.Pending;

        var subtask = new TaskEntity
        {
            ProjectId = parentTask.ProjectId,
            ParentTaskId = parentTask.Id,
            Title = request.Title,
            Description = request.Description,
            Priority = priority,
            Status = status,
            DueDate = request.DueDate,
            StartDate = request.StartDate,
            CompletedAt = status == TaskStatus.Completed ? DateTime.UtcNow : null,
            AssignedToId = request.AssignedToId,
            AssignedAt = request.AssignedToId.HasValue ? DateTime.UtcNow : null,
            CreatedById = _currentUserService.UserId,
            EstimatedHours = request.EstimatedHours,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Tasks.AddAsync(subtask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TaskDto
        {
            Id = subtask.Id,
            ProjectId = subtask.ProjectId,
            Title = subtask.Title,
            Description = subtask.Description,
            Priority = subtask.Priority.ToString(),
            Status = subtask.Status.ToString(),
            DueDate = subtask.DueDate,
            StartDate = subtask.StartDate,
            CompletedAt = subtask.CompletedAt,
            AssignedToId = subtask.AssignedToId,
            AssignedAt = subtask.AssignedAt,
            CreatedById = subtask.CreatedById,
            ParentTaskId = subtask.ParentTaskId,
            EstimatedHours = subtask.EstimatedHours,
            ActualHours = subtask.ActualHours,
            CreatedAt = subtask.CreatedAt,
            UpdatedAt = subtask.UpdatedAt
        };
    }
}
