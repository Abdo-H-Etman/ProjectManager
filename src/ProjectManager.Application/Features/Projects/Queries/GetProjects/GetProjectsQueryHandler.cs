using Application.Common.Interfaces;
using Application.Features.Projects.DTOs;
using AutoMapper;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetProjectsQueryHandler(IProjectRepository projectRepository, ICurrentUserService currentUserService, IMapper mapper)
    {
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.FindAsync(
            p => (string.IsNullOrWhiteSpace(request.Status) || p.Status.ToString() == request.Status) &&
                 (!request.IsArchived.HasValue || p.IsArchived == request.IsArchived.Value) &&
                 (string.IsNullOrWhiteSpace(request.SearchTerm) || p.Name.Contains(request.SearchTerm)
                 || (p.Description != null && p.Description.Contains(request.SearchTerm)))
                 && p.OwnerId == _currentUserService.UserId!.Value,
            cancellationToken);

        return _mapper.Map<IReadOnlyList<ProjectDto>>(projects);
    }
}
