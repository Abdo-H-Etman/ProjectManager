using MediatR;

namespace Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid Id) : IRequest;
