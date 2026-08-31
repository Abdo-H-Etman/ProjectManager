using Application.Common.Interfaces;
using Application.Features.Projects.DTOs;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectsQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IReadOnlyList<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.FindAsync(
            p => (!request.Status.HasValue || p.Status == request.Status.Value) &&
                 (!request.IsArchived.HasValue || p.IsArchived == request.IsArchived.Value) &&
                 (string.IsNullOrWhiteSpace(request.SearchTerm) || p.Name.Contains(request.SearchTerm) || (p.Description != null && p.Description.Contains(request.SearchTerm))),
            cancellationToken);

        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Status = p.Status,
            OwnerId = p.OwnerId,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            IsArchived = p.IsArchived,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();
    }
}
