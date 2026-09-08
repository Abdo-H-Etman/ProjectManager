using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;

namespace Application.Features.Tasks.Queries.GetSubtasks;

public class GetSubtasksQueryHandler : IRequestHandler<GetSubtasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetSubtasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetSubtasksQuery request, CancellationToken cancellationToken)
    {
        var parentTask = await _taskRepository.GetByIdAsync(request.ParentTaskId, cancellationToken);
        if (parentTask == null)
        {
            throw new NotFoundException(nameof(TaskEntity), request.ParentTaskId);
        }

        var subtasks = await _taskRepository.GetSubtasksAsync(request.ParentTaskId, cancellationToken);

        return subtasks.Select(t => new TaskDto
        {
            Id = t.Id,
            ProjectId = t.ProjectId,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority.ToString(),
            Status = t.Status.ToString(),
            DueDate = t.DueDate,
            StartDate = t.StartDate,
            CompletedAt = t.CompletedAt,
            AssignedToId = t.AssignedToId,
            AssignedAt = t.AssignedAt,
            CreatedById = t.CreatedById,
            ParentTaskId = t.ParentTaskId,
            EstimatedHours = t.EstimatedHours,
            ActualHours = t.ActualHours,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();
    }
}
