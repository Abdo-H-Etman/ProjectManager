using Domain.Common;
using Domain.Enums;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Domain.Entities.Models;

public class Task : BaseEntity
{
    public Guid ProjectId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? AssignedToId { get; set; }
    public DateTime? AssignedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public Guid? ParentTaskId { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }

    public Project Project { get; set; } = null!;
    public Task? ParentTask { get; set; }
    public ICollection<Task> SubTasks { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
}
