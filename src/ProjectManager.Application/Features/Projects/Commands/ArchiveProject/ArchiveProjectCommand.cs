using Application.Features.Projects.DTOs;
using MediatR;

namespace Application.Features.Projects.Commands.ArchiveProject;

public record ArchiveProjectCommand(Guid Id, bool IsArchived = true) : IRequest<ProjectDto>;
