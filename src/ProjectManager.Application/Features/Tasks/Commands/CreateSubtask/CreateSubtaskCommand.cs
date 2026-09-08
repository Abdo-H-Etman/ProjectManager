using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Commands.CreateSubtask;

public record CreateSubtaskCommand(
    Guid ParentTaskId,
    string Title,
    string? Description = null,
    string? Priority = null,
    string? Status = null,
    DateTime? DueDate = null,
    DateTime? StartDate = null,
    Guid? AssignedToId = null,
    decimal? EstimatedHours = null) : IRequest<TaskDto>;
