using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Queries.GetUnassignedTasks;

public class GetUnassignedTasksQueryHandler : IRequestHandler<GetUnassignedTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetUnassignedTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetUnassignedTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetUnassignedTasksAsync(request.ProjectId, cancellationToken);

        return tasks.Select(t => new TaskDto
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
