using Application.Features.Projects.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand : IRequest<ProjectDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Status { get; init; } = ProjectStatus.Active.ToString();
    public Guid? OwnerId { get; init; }
    public DateTime StartDate { get; init; } = DateTime.UtcNow;
    public DateTime? EndDate { get; init; }
}
