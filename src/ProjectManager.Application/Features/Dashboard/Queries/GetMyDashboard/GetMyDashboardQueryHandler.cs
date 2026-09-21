using Application.Common.Interfaces;
using Application.Features.Dashboard.DTOs;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using Domain.Enums;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Dashboard.Queries.GetMyDashboard;

public class GetMyDashboardQueryHandler : IRequestHandler<GetMyDashboardQuery, DashboardDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMyDashboardQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
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
            .Select(task => _mapper.Map<TaskDto>(task))
            .ToList();

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
