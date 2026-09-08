using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Queries.GetSubtasks;

public record GetSubtasksQuery(Guid ParentTaskId) : IRequest<IReadOnlyList<TaskDto>>;
