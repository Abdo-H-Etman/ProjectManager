using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Queries.GetOverdueTasks;

public record GetOverdueTasksQuery : IRequest<IReadOnlyList<TaskDto>>;
