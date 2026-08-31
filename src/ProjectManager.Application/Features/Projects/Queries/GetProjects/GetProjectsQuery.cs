using Application.Features.Projects.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjects;

public record GetProjectsQuery(
    ProjectStatus? Status = null,
    bool? IsArchived = null,
    string? SearchTerm = null) : IRequest<IReadOnlyList<ProjectDto>>;
