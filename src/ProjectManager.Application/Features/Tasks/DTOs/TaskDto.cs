using Domain.Enums;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.DTOs;

public class TaskDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? AssignedToId { get; set; }
    public DateTime? AssignedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public Guid? ParentTaskId { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class TaskDetailDto : TaskDto
{
    public string? ProjectName { get; set; }
    public int CommentCount { get; set; }
    public int SubTaskCount { get; set; }
}
