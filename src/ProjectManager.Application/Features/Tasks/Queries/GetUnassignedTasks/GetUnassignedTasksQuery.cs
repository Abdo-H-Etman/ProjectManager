using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Queries.GetUnassignedTasks;

public record GetUnassignedTasksQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<TaskDto>>;
