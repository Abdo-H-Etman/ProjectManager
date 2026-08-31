using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async System.Threading.Tasks.Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.Id, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.Id);
        }

        await _unitOfWork.Projects.DeleteAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
