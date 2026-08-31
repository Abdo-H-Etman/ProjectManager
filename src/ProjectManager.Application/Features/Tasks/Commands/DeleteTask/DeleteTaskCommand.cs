using MediatR;

namespace Application.Features.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid Id) : IRequest;
