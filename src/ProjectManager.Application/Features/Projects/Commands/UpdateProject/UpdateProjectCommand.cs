using Application.Features.Projects.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.Projects.Commands.UpdateProject;

public record UpdateProjectCommand : IRequest<ProjectDto>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Status { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool IsArchived { get; init; }
}
