using Application.Features.Tasks.DTOs;
using Domain.Enums;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand : IRequest<TaskDto>
{
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Priority { get; init; } = TaskPriority.Medium.ToString();
    public string Status { get; init; } = TaskStatus.Pending.ToString();
    public DateTime? DueDate { get; init; }
    public DateTime? StartDate { get; init; }
    public Guid? AssignedToId { get; init; }
    public Guid? CreatedById { get; init; }
    public Guid? ParentTaskId { get; init; }
    public decimal? EstimatedHours { get; init; }
}
