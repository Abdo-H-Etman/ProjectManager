using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTasks;

public record GetTasksQuery(
    Guid? ProjectId = null,
    string? Status = null,
    string? Priority = null,
    Guid? AssignedToId = null) : IRequest<IReadOnlyList<TaskDto>>;
