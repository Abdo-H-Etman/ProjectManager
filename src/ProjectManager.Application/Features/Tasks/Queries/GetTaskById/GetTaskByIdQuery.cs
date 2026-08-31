using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery(Guid Id) : IRequest<TaskDetailDto>;
