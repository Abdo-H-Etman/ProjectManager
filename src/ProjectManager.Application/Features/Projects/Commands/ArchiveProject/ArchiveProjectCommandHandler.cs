using Application.Common.Interfaces;
using Application.Common.Authorization;
using Application.Features.Projects.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Projects.Commands.ArchiveProject;

public class ArchiveProjectCommandHandler : IRequestHandler<ArchiveProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public ArchiveProjectCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<ProjectDto> Handle(ArchiveProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.Id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.Id);
        }

        AuthorizationRules.RequireOwnerOrAdmin(_currentUserService, project.OwnerId);

        project.IsArchived = request.IsArchived;
        if (request.IsArchived)
        {
            project.Status = ProjectStatus.Archived;
        }
        else if (project.Status == ProjectStatus.Archived)
        {
            project.Status = ProjectStatus.Active;
        }
        project.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Projects.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProjectDto>(project);
    }
}
