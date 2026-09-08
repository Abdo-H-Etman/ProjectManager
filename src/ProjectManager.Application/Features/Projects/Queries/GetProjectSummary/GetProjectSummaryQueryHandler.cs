using Application.Common.Interfaces;
using Application.Features.Projects.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using TaskPriority = Domain.Enums.TaskPriority;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Projects.Queries.GetProjectSummary;

public class GetProjectSummaryQueryHandler : IRequestHandler<GetProjectSummaryQuery, ProjectSummaryDto>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectSummaryQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectSummaryDto> Handle(GetProjectSummaryQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithTasksAsync(request.Id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.Id);
        }

        var totalTasks = project.Tasks.Count;
        var completedTasks = project.Tasks.Count(t => t.Status == TaskStatus.Completed);
        var progress = totalTasks > 0 ? Math.Round((double)completedTasks / totalTasks * 100.0, 2) : 0.0;

        var tasksByStatus = Enum.GetValues<TaskStatus>()
            .ToDictionary(s => s.ToString(), s => project.Tasks.Count(t => t.Status == s));

        var tasksByPriority = Enum.GetValues<TaskPriority>()
            .ToDictionary(p => p.ToString(), p => project.Tasks.Count(t => t.Priority == p));

        var estimatedHours = project.Tasks.Sum(t => t.EstimatedHours ?? 0);
        var actualHours = project.Tasks.Sum(t => t.ActualHours ?? 0);

        return new ProjectSummaryDto
        {
            ProjectId = project.Id,
            ProjectName = project.Name,
            Status = project.Status.ToString(),
            IsArchived = project.IsArchived,
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            ProgressPercentage = progress,
            TasksByStatus = tasksByStatus,
            TasksByPriority = tasksByPriority,
            TotalEstimatedHours = estimatedHours,
            TotalActualHours = actualHours
        };
    }
}
