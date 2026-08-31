using Application.Features.Tasks.DTOs;
using Domain.Enums;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand : IRequest<TaskDto>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskPriority Priority { get; init; }
    public TaskStatus Status { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime? StartDate { get; init; }
    public Guid? AssignedToId { get; init; }
    public decimal? EstimatedHours { get; init; }
    public decimal? ActualHours { get; init; }
}
