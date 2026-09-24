using Application.Common.Interfaces;
using Application.Common.Authorization;
using Application.Features.Projects.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.Id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.Id);
        }

        AuthorizationRules.RequireOwnerOrAdmin(_currentUserService, project.OwnerId);

        Enum.TryParse(request.Status, true, out ProjectStatus status);
        project.Name = request.Name;
        project.Description = request.Description;
        project.Status = status;
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.IsArchived = request.IsArchived;
        project.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Projects.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProjectDto>(project);
    }
}
