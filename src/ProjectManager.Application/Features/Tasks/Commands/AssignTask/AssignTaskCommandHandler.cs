using Application.Common.Interfaces;
using Application.Common.Authorization;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;

namespace Application.Features.Tasks.Commands.AssignTask;

public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public AssignTaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<TaskDto> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException(nameof(TaskEntity), request.Id);
        }

        AuthorizationRules.RequireOwnerOrAdmin(_currentUserService, task.Project.OwnerId);

        task.AssignedToId = request.AssignedToId;
        task.AssignedAt = request.AssignedToId.HasValue ? DateTime.UtcNow : null;
        task.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(task);
    }
}
