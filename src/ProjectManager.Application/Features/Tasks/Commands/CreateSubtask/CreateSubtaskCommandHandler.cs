using Application.Common.Interfaces;
using Application.Features.Tasks.DTOs;
using AutoMapper;
using Domain.Exceptions;
using MediatR;
using TaskEntity = Domain.Entities.Task;
using ProjectEntity = Domain.Entities.Project;
using TaskPriority = Domain.Enums.TaskPriority;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.CreateSubtask;

public class CreateSubtaskCommandHandler : IRequestHandler<CreateSubtaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateSubtaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TaskDto> Handle(CreateSubtaskCommand request, CancellationToken cancellationToken)
    {
        var parentTask = await _unitOfWork.Tasks.GetByIdAsync(request.ParentTaskId, cancellationToken);
        if (parentTask == null)
        {
            throw new NotFoundException(nameof(TaskEntity), request.ParentTaskId);
        }

        if (parentTask.IsDeleted)
        {
            throw new DeletedException(nameof(TaskEntity), request.ParentTaskId);
        }

        var project = await _unitOfWork.Projects.GetByIdAsync(parentTask.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException(nameof(ProjectEntity), parentTask.ProjectId);
        }

        if (project.IsDeleted)
        {
            throw new DeletedException(nameof(ProjectEntity), parentTask.ProjectId);
        }
        var priority = Enum.TryParse<TaskPriority>(request.Priority, true, out var parsedPriority)
            ? parsedPriority
            : TaskPriority.Medium;

        var status = Enum.TryParse<TaskStatus>(request.Status, true, out var parsedStatus)
            ? parsedStatus
            : TaskStatus.Todo;

        var subtask = new TaskEntity
        {
            ProjectId = parentTask.ProjectId,
            ParentTaskId = parentTask.Id,
            Title = request.Title,
            Description = request.Description,
            Priority = priority,
            Status = status,
            DueDate = request.DueDate,
            StartDate = request.StartDate,
            CompletedAt = status == TaskStatus.Completed ? DateTime.UtcNow : null,
            AssignedToId = request.AssignedToId,
            AssignedAt = request.AssignedToId.HasValue ? DateTime.UtcNow : null,
            CreatedById = _currentUserService.UserId,
            EstimatedHours = request.EstimatedHours,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Tasks.AddAsync(subtask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(subtask);
    }
}
