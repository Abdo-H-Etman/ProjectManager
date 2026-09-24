using Application.Common.Interfaces;
using Application.Common.Authorization;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async System.Threading.Tasks.Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.Id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.Id);
        }

        AuthorizationRules.RequireOwnerOrAdmin(_currentUserService, project.OwnerId);

        if (project.IsDeleted)
        {
            throw new DeletedException(nameof(Project), request.Id);
        }

        project.IsDeleted = true;
        project.DeletedAt = DateTime.UtcNow;
        _unitOfWork.Projects.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
