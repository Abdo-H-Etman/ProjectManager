using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Commands.AssignTask;

public record AssignTaskCommand(Guid Id, Guid? AssignedToId) : IRequest<TaskDto>;
