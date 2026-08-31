using Application.Common.Interfaces;
using Application.Features.Projects.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.Id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.Id);
        }

        Enum.TryParse(request.Status, true, out ProjectStatus status);
        project.Name = request.Name;
        project.Description = request.Description;
        project.Status = status;
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.IsArchived = request.IsArchived;
        project.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Projects.UpdateAsync(project, cancellationToken);
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
