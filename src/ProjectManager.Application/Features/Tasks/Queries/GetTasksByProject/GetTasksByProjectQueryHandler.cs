using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Queries.GetTasksByProject;

public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public GetTasksByProjectQueryHandler(ITaskRepository taskRepository, IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        var projectExists = await _projectRepository.ExistsAsync(request.ProjectId, cancellationToken);
        if (!projectExists)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        var status = Enum.TryParse<TaskStatus>(request.Status, true, out var parsedStatus)
            ? parsedStatus
            : (TaskStatus?)null;
        var priority = Enum.TryParse<Domain.Enums.TaskPriority>(request.Priority, true, out var parsedPriority)
            ? parsedPriority
            : (Domain.Enums.TaskPriority?)null;

        var tasks = await _taskRepository.GetTasksByProjectIdAsync(
            request.ProjectId,
            status,
            priority,
            request.AssignedToId,
            cancellationToken);

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
