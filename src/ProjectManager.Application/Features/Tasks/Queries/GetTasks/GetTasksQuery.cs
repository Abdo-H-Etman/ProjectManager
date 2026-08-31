using Application.Features.Tasks.DTOs;
using Domain.Enums;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Queries.GetTasks;

public record GetTasksQuery(
    Guid? ProjectId = null,
    TaskStatus? Status = null,
    TaskPriority? Priority = null,
    Guid? AssignedToId = null) : IRequest<IReadOnlyList<TaskDto>>;
