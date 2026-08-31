using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetTasksByFilterAsync(
            request.ProjectId,
            request.Status,
            request.Priority,
            request.AssignedToId,
            cancellationToken);

        return tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            ProjectId = t.ProjectId,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority,
            Status = t.Status,
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
