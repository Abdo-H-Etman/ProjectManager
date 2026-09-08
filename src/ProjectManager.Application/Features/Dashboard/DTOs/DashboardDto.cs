using Application.Features.Tasks.DTOs;

namespace Application.Features.Dashboard.DTOs;

public class DashboardDto
{
    public int TotalAssignedTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int PendingTasks { get; set; }
    public int ReviewTasks { get; set; }
    public int OverdueTasksCount { get; set; }
    public int DueSoonTasksCount { get; set; }
    public int ActiveProjectsCount { get; set; }
    public IReadOnlyList<TaskDto> UpcomingDeadlines { get; set; } = [];
}
