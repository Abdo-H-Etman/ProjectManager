using Application.Common.Interfaces;
using Application.Features.Dashboard.DTOs;
using Application.Features.Tasks.DTOs;
using Domain.Enums;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Dashboard.Queries.GetMyDashboard;

public class GetMyDashboardQueryHandler : IRequestHandler<GetMyDashboardQuery, DashboardDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyDashboardQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardDto> Handle(GetMyDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var tasks = await _unitOfWork.Tasks.GetTasksByFilterAsync(assignedToId: currentUserId, cancellationToken: cancellationToken);

        var now = DateTime.UtcNow;
        var sevenDaysFromNow = now.AddDays(7);

        var totalAssigned = tasks.Count;
        var completed = tasks.Count(t => t.Status == TaskStatus.Completed);
        var inProgress = tasks.Count(t => t.Status == TaskStatus.InProgress);
        var pending = tasks.Count(t => t.Status == TaskStatus.Pending);
        var review = tasks.Count(t => t.Status == TaskStatus.Review);

        var overdue = tasks.Count(t => t.DueDate != null && t.DueDate < now && t.Status != TaskStatus.Completed && t.Status != TaskStatus.Cancelled);
        var dueSoon = tasks.Count(t => t.DueDate != null && t.DueDate >= now && t.DueDate <= sevenDaysFromNow && t.Status != TaskStatus.Completed && t.Status != TaskStatus.Cancelled);

        var upcomingDeadlines = tasks
            .Where(t => t.DueDate != null && t.DueDate >= now && t.Status != TaskStatus.Completed && t.Status != TaskStatus.Cancelled)
            .OrderBy(t => t.DueDate)
            .Take(5)
            .Select(t => new TaskDto
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

        var activeProjects = await _unitOfWork.Projects.FindAsync(p => !p.IsArchived && p.Status == ProjectStatus.Active, cancellationToken);

        return new DashboardDto
        {
            TotalAssignedTasks = totalAssigned,
            CompletedTasks = completed,
            InProgressTasks = inProgress,
            PendingTasks = pending,
            ReviewTasks = review,
            OverdueTasksCount = overdue,
            DueSoonTasksCount = dueSoon,
            ActiveProjectsCount = activeProjects.Count,
            UpcomingDeadlines = upcomingDeadlines
        };
    }
}
