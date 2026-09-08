using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTasksByProject;

public record GetTasksByProjectQuery(
    Guid ProjectId,
    string? Status = null,
    string? Priority = null,
    Guid? AssignedToId = null) : IRequest<IReadOnlyList<TaskDto>>;
