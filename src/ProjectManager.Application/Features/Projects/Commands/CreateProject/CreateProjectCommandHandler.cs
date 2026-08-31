using Application.Common.Interfaces;
using Application.Features.Projects.DTOs;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateProjectCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        Enum.TryParse(request.Status, true, out ProjectStatus status);
        var ownerId = _currentUserService.UserId ?? request.OwnerId ?? Guid.NewGuid();

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Status = status,
            OwnerId = ownerId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Projects.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProjectDto
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
            UpdatedAt = project.UpdatedAt
        };
    }
}
