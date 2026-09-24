using Application.Common.Interfaces;
using Application.Common.Authorization;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateTaskCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        if (project.IsDeleted)
        {
            throw new DeletedException(nameof(Project), request.ProjectId);
        }

        AuthorizationRules.RequireOwnerOrAdmin(_currentUserService, project.OwnerId,
            "You can only create tasks in projects you own or administer.");

        var createdById = AuthorizationRules.RequireAuthenticatedUser(_currentUserService);

        Enum.TryParse(request.Priority, true, out TaskPriority priority);
        Enum.TryParse(request.Status, true, out TaskStatus status);
        var task = new TaskEntity
        {
            ProjectId = request.ProjectId,
            Title = request.Title,
            Description = request.Description,
            Priority = priority,
            Status = status,
            DueDate = request.DueDate,
            StartDate = request.StartDate,
            AssignedToId = request.AssignedToId,
            AssignedAt = request.AssignedToId.HasValue ? DateTime.UtcNow : null,
            CreatedById = createdById,
            ParentTaskId = request.ParentTaskId,
            EstimatedHours = request.EstimatedHours,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(task);
    }
}
