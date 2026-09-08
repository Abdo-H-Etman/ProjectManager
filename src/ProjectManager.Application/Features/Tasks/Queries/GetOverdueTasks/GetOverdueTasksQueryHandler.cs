using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Queries.GetOverdueTasks;

public class GetOverdueTasksQueryHandler : IRequestHandler<GetOverdueTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetOverdueTasksQueryHandler(ITaskRepository taskRepository, ICurrentUserService currentUserService)
    {
        _taskRepository = taskRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetOverdueTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetOverdueTasksAsync(_currentUserService.UserId, cancellationToken);

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
