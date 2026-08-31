using Application.Common.Interfaces;
using Application.Features.Projects.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDetailDto>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectByIdQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectDetailDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithTasksAsync(request.Id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.Id);
        }

        return new ProjectDetailDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            OwnerId = project.OwnerId,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            IsArchived = project.IsArchived,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            TaskCount = project.Tasks.Count,
            CompletedTaskCount = project.Tasks.Count(t => t.Status == TaskStatus.Completed)
        };
    }
}
