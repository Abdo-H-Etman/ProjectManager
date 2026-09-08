using Application.Features.Tasks.DTOs;
using MediatR;

namespace Application.Features.Tasks.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(Guid Id, string Status) : IRequest<TaskDto>;
